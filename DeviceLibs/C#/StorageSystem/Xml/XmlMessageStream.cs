using System;
using System.IO;
using System.Text;
using System.Threading;
using CareFusion.Lib.StorageSystem.Logging;
using Rowa.Lib.Log;

namespace CareFusion.Lib.StorageSystem.Xml
{
    /// <summary>
    /// Class which implements the logic to read and write UTF-8 encoded XML messages.
    /// </summary>
    public class XmlMessageStream : IDisposable
    {
        #region Constants

        /// <summary>
        /// Maximum supported length of a XML message in bytes.
        /// </summary>
        private const int MaxMessageLength = 100000000;

        /// <summary>
        /// Size of the read buffer to use when reading messages.
        /// </summary>
        private const int ReadBufferSize = 4194304;

        /// <summary>
        /// Template for the end indicator of a XML object message.
        /// </summary>
        private const string MessageEndTemplate = "</{0}>";

        #endregion

        #region Members

        /// <summary>
        /// The binary stream to use for reading and writing serialized objects.
        /// </summary>
        private Stream _stream = null;

        /// <summary>
        /// Reference to the message trace log file.
        /// </summary>
        private IWwi _messageTrace = null;

        /// <summary>
        /// Holds the XML message end tag as UTF-8 encoded byte array. 
        /// </summary>
        private byte[] _xmlEndTag = null;

        /// <summary>
        /// Read buffer to use when reading bytes from the underlying stream.
        /// </summary>
        private byte[] _readBuffer = new byte[ReadBufferSize];

        /// <summary>
        /// Overlapped buffer for unprocessed data of the last read call.
        /// </summary>
        private byte[] _overlappedBuffer = null;

        /// <summary>
        /// Event to cancel any blocking read and write operations.
        /// </summary>
        private ManualResetEvent _cancelEvent = new ManualResetEvent(false);

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlMessageStream"/> class.
        /// </summary>
        /// <param name="stream">The base stream to use for reading and writing XML messages.</param>
        /// <param name="xmlRootTag">The XML root tag of the XML messages.</param>
        /// <param name="traceDescription">The WWI trace description.</param>
        /// <param name="traceEndPoint">The WWI trace endpoint.</param>
        public XmlMessageStream(Stream stream, 
                                string xmlRootTag, 
                                string traceDescription = null,
                                string traceEndPoint = null)
        {
            if (stream == null)
            {
                throw new ArgumentException("Invalid stream specified.");
            }

            if (string.IsNullOrEmpty(xmlRootTag))
            {
                throw new ArgumentException("Invalid xmlRootTag specified.");
            }

            _stream = stream;
            _xmlEndTag = Encoding.UTF8.GetBytes(string.Format(MessageEndTemplate, xmlRootTag));

            if ((string.IsNullOrEmpty(traceDescription) == false) &&
                (string.IsNullOrEmpty(traceEndPoint) == false))
            {
                ushort tracePort = 0;
                int index = traceEndPoint.IndexOf(':');

                if ((index > 0) && (index < traceEndPoint.Length - 1))
                {
                    ushort.TryParse(traceEndPoint.Substring(index + 1), out tracePort);
                    traceEndPoint = traceEndPoint.Substring(0, index);
                }

                WwiLogIntercept.CreateBaseWwiLogger(traceDescription, traceEndPoint, tracePort);
                _messageTrace = WwiLogIntercept.GetSingleton();
            }
        }

        /// <summary>
        /// Writes the specified XML message to the underlying stream.
        /// </summary>
        /// <param name="xmlMessage">The XML message to write.</param>
        /// <param name="length">The length of the XML message to write.</param>
        /// <returns>
        ///   <c>true</c> if successful, <c>false</c> otherwise.
        /// </returns>
        public bool Write(byte[] xmlMessage, int length)
        {
            if (_cancelEvent.WaitOne(0))
            {
                return false;
            }

            if ((xmlMessage == null) || (length == 0))
            {
                return false;
            }

            IAsyncResult writeResult = _stream.BeginWrite(xmlMessage, 0, length, null, null);
            WaitHandle[] writeHandles = new WaitHandle[] { _cancelEvent, writeResult.AsyncWaitHandle };
            int waitResult = WaitHandle.WaitAny(writeHandles);

            if (writeHandles[waitResult] == _cancelEvent)
            {
                this.Info("Writing of XML message was cancelled.");
                return false;
            }

            _stream.EndWrite(writeResult);
            _stream.Flush();

            if (_messageTrace != null)
            {
                _messageTrace.LogMessage(xmlMessage, 0, length, false);
            }

            return true;
        }

        /// <summary>
        /// Reads the next XML message from the underlying stream.
        /// </summary>
        /// <param name="readTimeOutSeconds">The optional read time out seconds in seconds (0 means no timeout).</param>
        /// <returns>
        /// Read XML message if successful;<c>null</c> otherwise.
        /// </returns>
        public byte[] Read(int readTimeOutSeconds)
        {
            int numBytesRead = 0;
            int msgEndIndex = -1;
            byte[] collectBuffer = null;

            if (_overlappedBuffer != null)
            {
                msgEndIndex = _overlappedBuffer.GetIndexOf(_xmlEndTag);

                if (msgEndIndex > 0)
                {
                    msgEndIndex += _xmlEndTag.Length;
                    collectBuffer = new byte[msgEndIndex];
                    Array.Copy(_overlappedBuffer, collectBuffer, msgEndIndex);

                    if (_overlappedBuffer.Length > msgEndIndex)
                    {
                        byte[] newOverlappedBuffer = new byte[_overlappedBuffer.Length - msgEndIndex];
                        Array.Copy(_overlappedBuffer, msgEndIndex, newOverlappedBuffer, 0, newOverlappedBuffer.Length);
                        _overlappedBuffer = newOverlappedBuffer;
                    }
                    else
                    {
                        _overlappedBuffer = null;
                    }

                    if (_messageTrace != null)
                    {
                        _messageTrace.LogMessage(collectBuffer, 0, collectBuffer.Length, true);
                    }

                    return collectBuffer;
                }

                collectBuffer = new byte[_overlappedBuffer.Length];
                Array.Copy(_overlappedBuffer, collectBuffer, _overlappedBuffer.Length);
                _overlappedBuffer = null;
            }

            while ((numBytesRead = ReadChunk(readTimeOutSeconds)) > 0)
            {
                if (collectBuffer == null)
                {
                    collectBuffer = new byte[numBytesRead];
                }
                else
                {
                    Array.Resize(ref collectBuffer, collectBuffer.Length + numBytesRead);
                }

                if (collectBuffer.Length >= MaxMessageLength)
                {
                    this.Fatal("The received buffer exceeded the maximum of {0} bytes.", MaxMessageLength);
                    return null;
                }

                Array.Copy(_readBuffer, 0, collectBuffer, collectBuffer.Length - numBytesRead, numBytesRead);
                msgEndIndex = collectBuffer.GetIndexOf(_xmlEndTag);

                if (msgEndIndex == -1)
                {
                    continue;
                }

                msgEndIndex += _xmlEndTag.Length;

                if (collectBuffer.Length > msgEndIndex)
                {
                    _overlappedBuffer = new byte[collectBuffer.Length - msgEndIndex];
                    Array.Copy(collectBuffer, msgEndIndex, _overlappedBuffer, 0, _overlappedBuffer.Length);
                    Array.Resize(ref collectBuffer, msgEndIndex);
                }

                if (_messageTrace != null)
                {
                    _messageTrace.LogMessage(collectBuffer, 0, collectBuffer.Length, true);
                }

                return collectBuffer;
            }

            return null;
        }

        /// <summary>
        /// Cancels any currently running and blocking read or write operations.
        /// </summary>
        public void Cancel()
        {
            _cancelEvent.Set();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_messageTrace != null)
            {
                _messageTrace.Dispose();
            }

            _cancelEvent.Dispose();
        }

        /// <summary>
        /// Reads a chunk of bytes from the base stream into the read buffer.
        /// </summary>
        /// <param name="readTimeOutSeconds">Optional read timeout in seconds  (0 means no timeout).</param>
        /// <returns>
        /// Number of bytes read if successful; <c>0</c> otherwise.
        /// </returns>
        private int ReadChunk(int readTimeOutSeconds)
        {
            if (_cancelEvent.WaitOne(0))
            {
                return 0;
            }

            int waitResult = WaitHandle.WaitTimeout;
            IAsyncResult readResult = _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, null, null);
            WaitHandle[] readHandles = new WaitHandle[] { _cancelEvent, readResult.AsyncWaitHandle };

            if (readTimeOutSeconds == 0)
            {
                waitResult = WaitHandle.WaitAny(readHandles);
            }
            else
            {
                waitResult = WaitHandle.WaitAny(readHandles, readTimeOutSeconds * 1000);
            }

            if (waitResult == WaitHandle.WaitTimeout)
            {
                this.Error("Reading of content timed out after '{0}' seconds.", readTimeOutSeconds);
                return 0;
            }

            if (readHandles[waitResult] == _cancelEvent)
            {
                this.Info("Reading of content was cancelled.");
                return 0;
            }

            return _stream.EndRead(readResult);
        }

        #endregion

    }
}
