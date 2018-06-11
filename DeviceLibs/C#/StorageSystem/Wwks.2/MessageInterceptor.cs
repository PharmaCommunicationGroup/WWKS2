using CareFusion.Lib.StorageSystem.Wwks2.Messages;
using System;
using System.Threading;

namespace CareFusion.Lib.StorageSystem.Wwks2
{
    /// <summary>
    /// Class which contains all information about a WWKS 2.0 message interception.
    /// It is used to wait for the retrieval of a specific message.
    /// </summary>
    public class MessageInterceptor : IDisposable
    {
        #region Members

        /// <summary>
        /// ID of the message to intercept.
        /// </summary>
        private string _messageID;

        /// <summary>
        /// Type of the message to intercept.
        /// </summary>
        private Type _messageType;

        /// <summary>
        /// After successful interception the reference to the intercepted message.
        /// </summary>
        private MessageBase _message;

        /// <summary>
        /// Event which is triggered when the requested message was intercepted or the interception was cancelled.
        /// </summary>
        private ManualResetEvent _interceptionEvent = new ManualResetEvent(false);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the intercepted message object. 
        /// </summary>
        public MessageBase Message { get { return _message; } }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageInterceptor" /> class.
        /// </summary>
        /// <param name="messageID">ID of the message to intercept.</param>
        /// <param name="messageType">Type of the message to intercept.</param>
        public MessageInterceptor(string messageID, Type messageType)
        {
            if (messageType == null)
            {
                throw new ArgumentException("Invalid messageType specified.");
            }

            _messageID = messageID;
            _messageType = messageType;
            _message = null;
        }

        /// <summary>
        /// Checks whether the specified message is the one to intercept and processes it accordingly.
        /// </summary>
        /// <param name="message">The message to check for interception.</param>
        /// <returns><c>true</c> if the message was intercepted;<c>false</c> otherwise.</returns>
        public bool InterceptMessage(MessageBase message)
        {
            if (message == null)
            {
                return false;
            }

            if ((message.Id == _messageID) && (message.GetType() == _messageType))
            {
                _message = message;
                _interceptionEvent.Set();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Waits for the message interception notification.
        /// </summary>
        public void Wait()
        {
            _interceptionEvent.WaitOne();
            _interceptionEvent.Reset();
        }

        /// <summary>
        /// Waits for the message interception notification.
        /// </summary>
        /// <param name="timeoutMilliseconds">The wait timeout in milliseconds.</param>
        /// <returns><c>true</c> if wait finished successfully;<c>false</c> otherwise.</returns>
        public bool Wait(int timeoutMilliseconds)
        {
            return _interceptionEvent.WaitOne(timeoutMilliseconds);
        }

        /// <summary>
        /// Cancels pending wait operations for the interception of a message. 
        /// </summary>
        public void Cancel()
        {
            _interceptionEvent.Set();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _message = null;
            _messageType = null;
            _interceptionEvent.Dispose();
        }

        #endregion
    }
}
