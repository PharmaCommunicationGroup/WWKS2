using CareFusion.Lib.StorageSystem.Logging;
using CareFusion.Lib.StorageSystem.Wwks2.Messages;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.ArticleInformation;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Configuration;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Input;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.KeepAlive;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Output;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Sales;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Status;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Stock;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Task;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CareFusion.Lib.StorageSystem.Wwks2
{
    /// <summary>
    /// Class which continuously read message objects from a xml object stream
    /// and dispatches them to either active message interceptors or message arrival event listeners.  
    /// </summary>
    public class MessageDispatcher : IDisposable
    {
        #region Types

        /// <summary>
        /// Class which is used as argument for the MessageArrived event.
        /// </summary>
        public class MessageArrivedArgs : EventArgs
        {
            /// <summary>
            /// Message which has arrived.
            /// </summary>
            public MessageBase Message { get; set; }
        }

        #endregion

        #region Members

        /// <summary>
        /// The message object stream instance to read messages from.
        /// </summary>
        private XmlObjectStream _messageObjectStream;

        /// <summary>
        /// Thread which is used to continuously read message object from the message object stream.
        /// </summary>
        private Thread _messageReaderThread;

        /// <summary>
        /// Event which is used to synchronize the shutdown of the message reader thread.
        /// </summary>
        private ManualResetEvent _threadDownEvent = new ManualResetEvent(false);

        /// <summary>
        /// List of active message interceptors.
        /// </summary>
        private List<MessageInterceptor> _activeInterceptors = new List<MessageInterceptor>();

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private object _syncLock = new object();

        #endregion

        #region Events

        /// <summary>
        /// Event which is raised when a new message arrived that is not handled by any pending message interceptors.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Instance of MessageArrivedArgs with event details.</param>
        public event EventHandler MessageArrived;

        /// <summary>
        /// Event which is raised when the message reader detected a problem while reading messages from the stream.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Always null and not used here.</param>
        public event EventHandler MessageStreamDown;

        #endregion

        #region Methods

        /// <summary>
        /// Starts the message reading and dispatching on the specified message object stream.
        /// </summary>
        /// <param name="messageObjectStream">Message object stream to read the messages from.</param>
        public void Start(XmlObjectStream messageObjectStream)
        {
            if (messageObjectStream == null)
            {
                throw new ArgumentException("Invalid messageObjectStream specified.");
            }

            Stop();

            lock (_syncLock)
            {
                _threadDownEvent.Reset();
                _messageObjectStream = messageObjectStream;
                _messageReaderThread = new Thread(new ThreadStart(MessageReader));
                _messageReaderThread.Start();
            }
        }

        /// <summary>
        /// Stops the message reading and dispatching on the current message object stream.
        /// </summary>
        public void Stop()
        {
            lock (_syncLock)
            {
                if (_messageReaderThread == null)
                {
                    return;
                }

                _messageObjectStream.Cancel();
                _threadDownEvent.WaitOne();

                _messageObjectStream = null;
                _messageReaderThread = null;
            }
        }

        /// <summary>
        /// Sends the specified message envelope and waits for the response in form of the specified message type.
        /// </summary>
        /// <param name="messageEnvelope">The message envelope to send.</param>
        /// <param name="messageId">The message identifier to use while waiting for the response.</param>
        /// <param name="responseType">The type of the response message to wait for.</param>
        /// <param name="waitTimeoutMilliseconds">The wait timeout milliseconds.</param>
        /// <returns>
        /// Requested response message if it was received; null otherwise.
        /// </returns>
        public MessageBase SendAndWaitForResponse(object messageEnvelope, 
                                                  string messageId, 
                                                  Type responseType,
                                                  int waitTimeoutMilliseconds = 0)
        {
            if (messageEnvelope == null)
            {
                throw new ArgumentException("Invalid messageEnvelope specified.");
            }

            if (responseType == null)
            {
                throw new ArgumentException("Invalid responseType specified.");
            }

            MessageInterceptor interceptor = null;

            try
            {
                lock (_syncLock)
                {
                    if (_messageObjectStream == null)
                    {
                        return null;
                    }

                    interceptor = new MessageInterceptor(messageId, responseType);
                    AddInterceptor(interceptor);

                    if (_messageObjectStream.Write(messageEnvelope) == false)
                    {
                        throw new ApplicationException(string.Format("Sending '{0}' to storage system failed.", messageEnvelope.GetType().Name));
                    }
                }

                if (waitTimeoutMilliseconds == 0)
                {
                    interceptor.Wait();
                    return interceptor.Message;
                }
                else
                {
                    if (interceptor.Wait(waitTimeoutMilliseconds))
                    {
                        return interceptor.Message;
                    }

                    return null;
                }
            }
            finally
            {
                if (interceptor != null)
                {
                    RemoveInterceptor(interceptor);
                    interceptor.Dispose();
                }
            }
        }

        /// <summary>
        /// Adds the specified interceptor to the internal interceptor list.
        /// </summary>
        /// <param name="interceptor">The interceptor to add.</param>
        public void AddInterceptor(MessageInterceptor interceptor)
        {
            lock (_activeInterceptors)
            {
                _activeInterceptors.Add(interceptor);
            }
        }

        /// <summary>
        /// Removes the specified interceptor from the internal interceptor list.
        /// </summary>
        /// <param name="interceptor">The interceptor to remove.</param>
        public void RemoveInterceptor(MessageInterceptor interceptor)
        {
            lock (_activeInterceptors)
            {
                _activeInterceptors.Remove(interceptor);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// The thread main of the message reading and processing thread.
        /// </summary>
        private void MessageReader()
        {
            try
            {
                object messageEnvelope = null;

                while ((messageEnvelope = _messageObjectStream.Read()) != null)
                {
                    MessageBase message = GetMessageFromEnvelope(messageEnvelope);

                    if (message == null)
                    {
                        this.Error("Received unexpected message envelope of type '{0}' -> ignore it.", messageEnvelope.GetType().Name);
                        continue;
                    }

                    if (message.GetType().Name == "KeepAliveRequest")
                    {
                        var response = new KeepAliveResponseEnvelope()
                        {
                            KeepAliveResponse = new KeepAliveResponse()
                            {
                                Id = message.Id,
                                Source = message.Destination,
                                Destination = message.Source
                            }
                        };

                        _messageObjectStream.Write(response);
                        continue;
                    }

                    lock (_activeInterceptors)
                    {
                        foreach (var interceptor in _activeInterceptors)
                        {
                            if (interceptor.InterceptMessage(message))
                            {
                                message = null;
                                break;
                            }
                        }
                    }

                    if ((message != null) && (this.MessageArrived != null))
                    {
                        this.MessageArrived(this, new MessageArrivedArgs() { Message = message });
                    }
                }   
            }
            catch (Exception ex)
            {
                this.Error("Reading and dispatching messages failed!", ex);
            }
            finally
            {
                lock (_activeInterceptors)
                {
                    foreach (var interceptor in _activeInterceptors)
                    {
                        interceptor.Cancel();
                    }
                }

                _threadDownEvent.Set();
            }

            try
            {
                if (this.MessageStreamDown != null)
                {
                    this.MessageStreamDown(this, null);
                }   
            }
            catch (Exception ex)
            {
                this.Error("Throwing final message stream down event failed!", ex);
            }                   
        }

        /// <summary>
        /// Extracts the message object from the specified message envelope.
        /// </summary>
        /// <param name="messageEnvelope">The message envelope to extract the message object from.</param>
        /// <returns>The extracted message object or null.</returns>
        private MessageBase GetMessageFromEnvelope(object messageEnvelope)
        {
            if (messageEnvelope == null)
            {
                return null;
            }

            switch (messageEnvelope.GetType().Name)
            {
                case "InputRequestEnvelope": return ((InputRequestEnvelope)messageEnvelope).InputRequest;
                case "InputMessageEnvelope": return ((InputMessageEnvelope)messageEnvelope).InputMessage;
                case "OutputResponseEnvelope": return ((OutputResponseEnvelope)messageEnvelope).OutputResponse;
                case "OutputMessageEnvelope": return ((OutputMessageEnvelope)messageEnvelope).OutputMessage;
                case "StatusResponseEnvelope": return ((StatusResponseEnvelope)messageEnvelope).StatusResponse;
                case "StockInfoResponseEnvelope": return ((StockInfoResponseEnvelope)messageEnvelope).StockInfoResponse;
                case "StockInfoMessageEnvelope": return ((StockInfoMessageEnvelope)messageEnvelope).StockInfoMessage;
                case "ArticleMasterSetResponseEnvelope": return ((ArticleMasterSetResponseEnvelope)messageEnvelope).ArticleMasterSetResponse;
                case "StockDeliverySetResponseEnvelope": return ((StockDeliverySetResponseEnvelope)messageEnvelope).StockDeliverySetResponse;
                case "TaskCancelResponseEnvelope": return ((TaskCancelResponseEnvelope)messageEnvelope).TaskCancelResponse;
                case "TaskInfoResponseEnvelope": return ((TaskInfoResponseEnvelope)messageEnvelope).TaskInfoResponse;
                case "KeepAliveRequestEnvelope": return ((KeepAliveRequestEnvelope)messageEnvelope).KeepAliveRequest;
                case "KeepAliveResponseEnvelope": return ((KeepAliveResponseEnvelope)messageEnvelope).KeepAliveResponse;  
                case "InitiateInputResponseEnvelope": return ((InitiateInputResponseEnvelope)messageEnvelope).InitiateInputResponse;
                case "InitiateInputMessageEnvelope": return ((InitiateInputMessageEnvelope)messageEnvelope).InitiateInputMessage;
                case "ConfigurationGetResponseEnvelope": return ((ConfigurationGetResponseEnvelope)messageEnvelope).ConfigurationGetResponse;
                case "StockLocationInfoResponseEnvelope": return ((StockLocationInfoResponseEnvelope)messageEnvelope).StockLocationInfoResponse;
                // digitalshelf
                case "ArticleInfoRequestEnvelope": return ((ArticleInfoRequestEnvelope)messageEnvelope).ArticleInfoRequest;
                case "ArticleInfoResponseEnvelope": return ((ArticleInfoResponseEnvelope)messageEnvelope).ArticleInfoResponse;
                case "ArticlePriceRequestEnvelope": return ((ArticlePriceRequestEnvelope)messageEnvelope).ArticlePriceRequest;
                case "ArticlePriceResponseEnvelope": return ((ArticlePriceResponseEnvelope)messageEnvelope).ArticlePriceResponse;
                case "ArticleSelectedMessageEnvelope": return ((ArticleSelectedMessageEnvelope)messageEnvelope).ArticleSelectedMessage;
                case "ShoppingCartRequestEnvelope": return ((ShoppingCartRequestEnvelope)messageEnvelope).ShoppingCartRequest;
                case "ShoppingCartResponseEnvelope": return ((ShoppingCartResponseEnvelope)messageEnvelope).ShoppingCartResponse;
                case "ShoppingCartUpdateMessageEnvelope": return ((ShoppingCartUpdateMessageEnvelope)messageEnvelope).ShoppingCartUpdateMessage;
                case "ShoppingCartUpdateRequestEnvelope": return ((ShoppingCartUpdateRequestEnvelope)messageEnvelope).ShoppingCartUpdateRequest;
                case "ShoppingCartUpdateResponseEnvelope": return ((ShoppingCartUpdateResponseEnvelope)messageEnvelope).ShoppingCartUpdateResponse;
                case "StockInfoRequestEnvelope": return ((StockInfoRequestEnvelope)messageEnvelope).StockInfoRequest;
            }

            return null;
        }

        #endregion

        
    }
}
