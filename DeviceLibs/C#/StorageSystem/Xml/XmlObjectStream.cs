using CareFusion.Lib.StorageSystem.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Xml
{
    /// <summary>
    /// Class which implements a stream that allows to read and write objects from
    /// a binary stream which are automatically serialized and deserialized.
    /// </summary>
    public class XmlObjectStream : IDisposable
    {
        #region Constants

        /// <summary>
        /// Regular expression to easily extract the serialized object type out of a XML object message.
        /// </summary>
        private const string MessageObjectTypeRegex = @"\<{0}[^\>]*\>\s*\<\s*(?<objectType>[^(\>|\s|\/)]+)";

        /// <summary>
        /// The maximum length of the a message type descriptor.
        /// </summary>
        private const int TypeDescriptorMaxLength = 125;

        #endregion

        #region Members

        /// <summary>
        /// The xml message stream to use for reading and writing xml object messages.
        /// </summary>
        private XmlMessageStream _stream = null;

        /// <summary>
        /// XML writer settings to use during serialization.
        /// </summary>
        private readonly XmlWriterSettings _xmlWriterSettings = null;

        /// <summary>
        /// XML Serializer settings to use during serialization.
        /// </summary>
        private readonly XmlSerializerNamespaces _xmlSerializerNamespaces = null;

        /// <summary>
        /// Regular expression to use for parse incomming XML content for the message type.
        /// </summary>
        private readonly Regex _messageObjectTypeRegex = null;

        /// <summary>
        /// Map of XmlSerializer instances and their according types used for reading objects.
        /// </summary>
        private Dictionary<string, XmlSerializer> _readSerializerMap = new Dictionary<string, XmlSerializer>();

        /// <summary>
        /// Map of XmlSerializer instances and their according types used for writing objects.
        /// </summary>
        private Dictionary<string, XmlSerializer> _writeSerializerMap = new Dictionary<string, XmlSerializer>();

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlObjectStream" /> class.
        /// </summary>
        /// <param name="stream">Base stream to use for reading and writing objects.</param>
        /// <param name="xmlRootTag">Name of the XML root tag of every object message.</param>
        /// <param name="traceDescription">The WWI trace description.</param>
        /// <param name="traceEndPoint">The WWI trace endpoint.</param>
        public XmlObjectStream(Stream stream, 
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

            _xmlWriterSettings = new XmlWriterSettings();
            _xmlWriterSettings.Encoding = new UTF8Encoding(false);
            _xmlWriterSettings.OmitXmlDeclaration = true;
            _xmlSerializerNamespaces = new XmlSerializerNamespaces();
            _xmlSerializerNamespaces.Add(string.Empty, string.Empty);
            _messageObjectTypeRegex = new Regex(string.Format(MessageObjectTypeRegex, xmlRootTag), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _stream = new XmlMessageStream(stream, xmlRootTag, traceDescription, traceEndPoint);
        }

        /// <summary>
        /// Adds the specified type to the list of supported XmlSerializer types.
        /// </summary>
        /// <param name="type">The type to add as supported type.</param>
        /// <param name="alternateTypeName">Alternate name for the specified type.</param>
        public void AddSupportedType(Type type, string alternateTypeName = null)
        {
            if (type == null)
            {
                throw new ArgumentException("Invalid type specified.");
            }

            string typeKey = alternateTypeName != null ? alternateTypeName.ToLower() : type.Name.ToLower();

            lock (_readSerializerMap)
            {
                if (_readSerializerMap.ContainsKey(typeKey))
                {
                    _readSerializerMap.Remove(typeKey);
                }

                _readSerializerMap.Add(typeKey, new XmlSerializer(type));
            }

            typeKey = type.Name.ToLower();

            lock (_writeSerializerMap)
            {
                if (_writeSerializerMap.ContainsKey(typeKey))
                {
                    _writeSerializerMap.Remove(typeKey);
                }

                _writeSerializerMap.Add(typeKey, new XmlSerializer(type));
            }
        }

        /// <summary>
        /// Serializes the specified object into XML and writes it to the base stream.
        /// </summary>
        /// <param name="messageObject">The message object to serialize.</param>
        /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
        public bool Write(object messageObject)
        {
            if (messageObject == null)
            {
                return false;
            }

            try
            {
                lock (_writeSerializerMap)
                {
                    if (_writeSerializerMap.Count == 0)
                    {
                        return false;
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (XmlWriter xw = XmlWriter.Create(ms, _xmlWriterSettings))
                        {
                            XmlSerializer serializer = _writeSerializerMap[messageObject.GetType().Name.ToLower()];
                            serializer.Serialize(xw, messageObject, _xmlSerializerNamespaces);
                            xw.Flush(); ms.Flush();
                            return _stream.Write(ms.GetBuffer(), Convert.ToInt32(ms.Length));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Error("Writing object of type '{0}' failed.", ex, messageObject.GetType().FullName);
            }

            return false;
        }

        /// <summary>
        /// Reads a new serialized message object from the base stream and deserializes it.
        /// </summary>
        /// <param name="readTimeOutSeconds">Optional read timeout in seconds  (0 means no timeout).</param>
        /// <returns>
        /// The deserialized object if successful, <c>null</c> otherwise.
        /// </returns>
        public object Read(int readTimeOutSeconds = 0)
        {
            try
            {
                lock (_readSerializerMap)
                {
                    if (_readSerializerMap.Count == 0)
                    {

                        return false;
                    }

                    XmlSerializer serializer = null;
                    byte[] message = null;

                    do
                    {
                        if ((message = _stream.Read(readTimeOutSeconds)) == null)
                        {
                            return null;
                        }

                        int descriptorLength = (message.Length < TypeDescriptorMaxLength) ? message.Length : TypeDescriptorMaxLength;
                        string typeDescriptor = Encoding.UTF8.GetString(message, 0, descriptorLength);
                        Match objectTypeMatch = _messageObjectTypeRegex.Match(typeDescriptor);
                        string messageObjectType = objectTypeMatch.Groups["objectType"].Value;

                        if ((objectTypeMatch.Success == false) || (string.IsNullOrWhiteSpace(messageObjectType)))
                        {
                            this.Error("The read object does not have a valid type.\r\n{0}\r\n", typeDescriptor);
                            return null;
                        }

                        messageObjectType = messageObjectType.Trim().ToLower();

                        if (_readSerializerMap.ContainsKey(messageObjectType) == false)
                        {
                            this.Error("The object type '{0}' is not supported.", messageObjectType);
                        }
                        else
                        {
                            serializer = _readSerializerMap[messageObjectType];
                        }
                    }
                    while (serializer == null);

                    using (MemoryStream ms = new MemoryStream(message))
                    {
                        return serializer.Deserialize(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Error("Reading serialized object failed.", ex);
            }

            return null;
        }

        /// <summary>
        /// Cancels any currently running and blocking read or write operations.
        /// This method will block and not return until all outstanding operations have finished.
        /// </summary>
        public void Cancel()
        {
            // cancel blocking operations
            _stream.Cancel();

            // wait until all locks are free again
            lock (_writeSerializerMap)
            {
                lock (_readSerializerMap)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_writeSerializerMap)
            {
                lock (_readSerializerMap)
                {
                    _stream.Dispose();
                    _writeSerializerMap.Clear();
                    _readSerializerMap.Clear();
                }
            }
        }

        #endregion
    }
}
