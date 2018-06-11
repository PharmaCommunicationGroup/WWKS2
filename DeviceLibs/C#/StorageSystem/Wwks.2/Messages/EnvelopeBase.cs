using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages
{
    /// <summary>
    /// Class which defines the envelope for every WWKS 2.0 message.
    /// </summary>
    public class EnvelopeBase
    {
        #region Properties

        [XmlAttribute]
        public string Version { get; set; }

        [XmlAttribute]
        public string TimeStamp { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvelopeBase"/> class.
        /// </summary>
        public EnvelopeBase()
        {
            // set defaults
            this.Version = "2.0";
            this.TimeStamp = TypeConverter.ConvertDateTime(DateTime.UtcNow);
        }
    }
}
