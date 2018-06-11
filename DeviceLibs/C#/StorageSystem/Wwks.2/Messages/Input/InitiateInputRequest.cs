using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Stock;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CareFusion.Lib.StorageSystem.Logging;
using System.Threading;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 InitiateInputRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class InitiateInputRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public InitiateInputRequest InitiateInputRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 InitiateInputRequest message.
    /// </summary>
    public class InitiateInputRequest : MessageBase, IInitiateInputRequest
    {
        #region Members

        /// <summary>
        /// The message object stream to use for sending the requests.
        /// </summary>
        private XmlObjectStream _messageOjectStream;

        /// <summary>
        /// The message dispatcher to use for message interception.
        /// </summary>
        private MessageDispatcher _messageDispatcher;

        /// <summary>
        /// The delivery number of the initiated input.
        /// </summary>
        private string _deliveryNumber;

        /// <summary>
        /// The current state of the output process.
        /// </summary>
        private InitiateInputRequestState _currentState = InitiateInputRequestState.Created;

        /// <summary>
        /// Holds the list of currently processed article information.
        /// </summary>
        private List<Article> _inputArticles = new List<Article>();

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private object _syncLock = new object();

        #endregion

        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string IsNewDelivery { get; set; }

        [XmlAttribute]
        public string SetPickingIndicator { get; set; }

        [XmlElement]
        public Details Details { get; set; }

        [XmlElement]
        public Article Article { get; set; }

        #endregion

        #region IInitiateInputProcess Specific Properties

        /// <summary>
        /// Gets the unique identifier number which is used to identify an initiated input process.
        /// </summary>
        [XmlIgnore]
        int IInitiateInputRequest.Id
        {
            get { return TypeConverter.ConvertInt(this.Id); }
        }

        /// <summary>
        /// Gets the destination identifier of the storage system that should start the input process.
        /// </summary>
        [XmlIgnore]
        int IInitiateInputRequest.Destination
        {
            get { return this.Destination; }
        }

        /// <summary>
        /// Gets the current state of the initiated input process.
        /// </summary>
        [XmlIgnore]
        InitiateInputRequestState IInitiateInputRequest.State
        {
            get
            {
                lock (_syncLock)
                {
                    return _currentState;
                }
            }
        }

        /// <summary>
        /// If the initiated pack input is part of a new stock delivery, this property
        /// specifies the delivery number which will be used during pack input.
        /// If this property is null, this pack input is NOT part of a new delivery.
        /// </summary>
        [XmlIgnore]
        string IInitiateInputRequest.DeliveryNumber
        {
            get 
            {
                return _deliveryNumber;
            }
        }

        /// <summary>
        /// Flag whether the storage system will set a so-called picking 
        /// indicator during the initiated input to enforce a redefinition 
        /// of the requested pack articles as storage system capable articles.
        /// This flag is usually used when the new storage system 
        /// gets filled for the very first time to realize a kind
        /// of first-time synchronization between the IT system and 
        /// the storage system according to which articles are "storage
        /// system articles".
        /// </summary>
        [XmlIgnore]
        bool IInitiateInputRequest.PickingIndicator
        {
            get { return string.IsNullOrEmpty(this.SetPickingIndicator) ? false : bool.Parse(this.SetPickingIndicator); }
        }

        /// <summary>
        /// Gets the number which defines the input source that should be used 
        /// to transfer the pack into the storage system. 
        /// </summary>
        [XmlIgnore]
        int IInitiateInputRequest.InputSource
        {
            get 
            { 
                if (this.Details == null)
                {
                    return 0;
                }
                
                if (string.IsNullOrEmpty(this.Details.InputSource))
                {
                    return 0;
                }

                return int.Parse(this.Details.InputSource);
            }
        }

        /// <summary>
        /// Gets the detailed information according to the part of the input source 
        /// which is used to transfer the pack into the storage system (e.g. belt number).
        /// </summary>
        [XmlIgnore]
        int IInitiateInputRequest.InputPoint
        {
            get
            {
                if (this.Details == null)
                {
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Details.InputPoint))
                {
                    return 0;
                }

                return int.Parse(this.Details.InputPoint);
            }
        }

        /// <summary>
        /// Gets the list of packs that are ready to be input.
        /// </summary>
        [XmlIgnore]
        IPack[] IInitiateInputRequest.InputPacks
        {
            get 
            {
                if ((this.Article != null) &&
                    (this.Article.Pack != null))
                {
                    return this.Article.Pack.ToArray();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the information of the articles and the packs that have been processed during
        /// the input. This property is set after the input process finished.
        /// </summary>
        [XmlIgnore]
        IArticle[] IInitiateInputRequest.InputArticles
        {
            get 
            { 
                lock (_syncLock) 
                { 
                    return _inputArticles.ToArray(); 
                } 
            }
        }

        #endregion

        #region IInitiateInputProcess Events

        /// <summary>
        /// Event which is raised when the initiated input process finished execution.
        /// </summary>
        /// <param name="sender">The initiated input process instance which raised the event.</param>
        /// <param name="e">Not used. Always null.</param>
        public event EventHandler Finished;

        #endregion

        #region IInitiateInputProcess Methods

        /// <summary>
        /// Adds a new pack that is ready for input.
        /// </summary>
        /// <param name="scanCode">Raw scan code of the pack.</param>
        /// <param name="batchNumber">Optional batch number of the pack.</param>
        /// <param name="externalId">Optional external identifier of the pack.</param>
        /// <param name="expiryDate">Optional expiry date of the pack.</param>
        /// <param name="subItemQuantity">Optional number of elements (e.g. pills or ampoules) which are currently in this pack.</param>
        /// <param name="depth">Optional depth of the pack in mm.</param>
        /// <param name="width">Optional width of the pack in mm.</param>
        /// <param name="height">Optional height of the pack in mm.</param>
        /// <param name="shape">Optional shape of the pack.</param>
        /// <param name="stockLocationId">Optional stock location to use for the pack.</param>
        /// <param name="machineLocation">Optional machine location to use for the pack.</param>
        void IInitiateInputRequest.AddInputPack(string scanCode, 
                                                string batchNumber,
                                                string externalId,
                                                DateTime? expiryDate, 
                                                int subItemQuantity, 
                                                int depth,
                                                int width, 
                                                int height, 
                                                PackShape shape,
                                                string stockLocationId,
                                                string machineLocation)
        {
            if (this.Article == null)
            {
                this.Article = new Article()
                {
                    Id = null,
                    Name = null,
                    DosageForm = null,
                    PackagingUnit = null,
                    Pack = new List<Pack>()
                };
            }

            this.Article.Pack.Add(new Pack()
            {
                ScanCode = TextConverter.EscapeInvalidXmlChars(scanCode),
                BatchNumber = TextConverter.EscapeInvalidXmlChars(batchNumber),
                ExternalId = TextConverter.EscapeInvalidXmlChars(externalId),
                DeliveryNumber = TextConverter.EscapeInvalidXmlChars(_deliveryNumber),
                ExpiryDate = expiryDate.HasValue ? TypeConverter.ConvertDate(expiryDate.Value) : null,
                SubItemQuantity = subItemQuantity.ToString(),
                Depth = depth.ToString(),
                Width = width.ToString(),
                Height = height.ToString(),
                Shape = shape.ToString(),
                StockLocationId = TextConverter.EscapeInvalidXmlChars(stockLocationId),
                MachineLocation = TextConverter.EscapeInvalidXmlChars(machineLocation)
            });
        }

        /// <summary>
        /// Requests the error detailes for a the specified processed pack.
        /// </summary>
        /// <param name="pack">The pack to get the error detailes for.</param>
        /// <param name="errorType">On successful return the type of error that occurred during input.</param>
        /// <param name="errorText">On successful return the additional text of the error that occurred during input.</param>
        /// <returns>
        ///   <c>true</c> if the specified pack had an error during input;<c>false</c> otherwise.
        /// </returns>
        bool IInitiateInputRequest.GetProcessedPackError(IPack pack, out InputErrorType errorType, out string errorText)
        {
            errorType = InputErrorType.None;
            errorText = string.Empty;

            if (pack == null)
            {
                return false;
            }

            if ((pack is Pack) == false)
            {
                return false;
            }

            var error = ((Pack)pack).Error;

            if (error == null)
            {
                return false;
            }

            if (Enum.TryParse<InputErrorType>(error.Type, out errorType) == false)
            {
                errorType = InputErrorType.Rejected;
            }

            errorText = TextConverter.UnescapeInvalidXmlChars(error.Text);
            return true;
        }

        /// <summary>
        /// Initiates the input process by sending the according request to the storage system.
        /// </summary>
        void IInitiateInputRequest.Start()
        {
            lock (_syncLock)
            {
                if (_currentState != InitiateInputRequestState.Created)
                {
                    return;
                }
            }

            var request = new InitiateInputRequestEnvelope() { InitiateInputRequest = this };
            var initiateInputMessageInterceptor = new MessageInterceptor(this.Id, typeof(InitiateInputMessage));
            _messageDispatcher.AddInterceptor(initiateInputMessageInterceptor);

            var response = (InitiateInputResponse)_messageDispatcher.SendAndWaitForResponse(request,
                                                                                            this.Id,
                                                                                            typeof(InitiateInputResponse));

            if (response == null)
            {
                _messageDispatcher.RemoveInterceptor(initiateInputMessageInterceptor);
                initiateInputMessageInterceptor.Dispose();
                throw new ArgumentException("Waiting for the message 'InitiateInputResponse' failed.");
            }

            if ((response.Details == null) ||
                (Enum.TryParse<InitiateInputRequestState>(response.Details.Status, out _currentState) == false))
            {
                _currentState = InitiateInputRequestState.Unknown;
            }

            if (_currentState != InitiateInputRequestState.Accepted)
            {
                if (this.Finished != null)
                {
                    this.Trace("Raising event 'Finished'.");
                    this.Finished(this, new EventArgs());
                }
            }
            else
            {
                this.Details.InputPoint = (response.Details != null) ? response.Details.InputPoint : "0";

                if (response.Article != null)
                {
                    lock (_syncLock)
                    {
                        _inputArticles.AddRange(response.Article);
                    }
                }

                // wait for completion
                if (ThreadPool.QueueUserWorkItem(new WaitCallback(WaitForInitiateInputMessage),
                                                 initiateInputMessageInterceptor) == false)
                {
                    throw new ApplicationException("Starting observation thread failed.");
                }
            }
        }

        #endregion

        #region Additional Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="InitiateInputRequest"/> class.
        /// </summary>
        public InitiateInputRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitiateInputRequest" /> class.
        /// </summary>
        /// <param name="messageOjectStream">The message object stream to use for sending the requests.</param>
        /// <param name="messageDispatcher">The message dispatcher to use for message interception.</param>
        /// <param name="deliveryNumber">The delivery number to use for the initiated input.</param>
        public InitiateInputRequest(XmlObjectStream messageOjectStream, MessageDispatcher messageDispatcher, string deliveryNumber)
        {
            if (messageOjectStream == null)
            {
                throw new ArgumentException("Invalid messageOjectStream specified.");
            }

            if (messageDispatcher == null)
            {
                throw new ArgumentException("Invalid messageDispatcher specified.");
            }

            _messageOjectStream = messageOjectStream;
            _messageDispatcher = messageDispatcher;
            _deliveryNumber = deliveryNumber;
            this.IsNewDelivery = (deliveryNumber != null).ToString();
            this.Details = new Details();
        }

        /// <summary>
        /// Asynchronous thread which waits for the completion of the specified initiated input message interceptor.
        /// </summary>
        /// <param name="messageInterceptor">The message interceptor to wait for.</param>
        private void WaitForInitiateInputMessage(object messageInterceptor)
        {
            try
            {
                using (var interceptor = messageInterceptor as MessageInterceptor)
                {
                    interceptor.Wait();
                    _messageDispatcher.RemoveInterceptor(interceptor);

                    var inputMessage = (InitiateInputMessage)interceptor.Message;

                    if (inputMessage != null)
                    {
                        lock (_syncLock)
                        {
                            if (inputMessage.Article != null)
                            {
                                _inputArticles = inputMessage.Article;
                            }

                            if (Enum.TryParse<InitiateInputRequestState>(inputMessage.Details.Status, out _currentState) == false)
                            {
                                _currentState = InitiateInputRequestState.Unknown;
                            }
                        }
                    }
                    else
                    {
                        this.Error("Waiting for the message 'InitiateInputMessage' failed.");

                        lock (_syncLock)
                        {
                            _currentState = InitiateInputRequestState.Unknown;
                        }
                    }

                    if (this.Finished != null)
                    {
                        this.Trace("Raising event 'Finished'.");
                        this.Finished(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                this.Error("Receiving and processing 'InitiateInputMessage' failed!", ex);
            }
        }

        #endregion
    }
}
