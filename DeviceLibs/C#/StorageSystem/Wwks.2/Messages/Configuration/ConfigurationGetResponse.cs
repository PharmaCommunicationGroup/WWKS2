using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Configuration
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ConfigurationGetResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ConfigurationGetResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ConfigurationGetResponse ConfigurationGetResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ConfigurationGetResponse message.
    /// </summary>
    public class ConfigurationGetResponse : MessageBase
    {
        #region Properties

        [XmlIgnore]
        public string RawContent { get; set; }

        [XmlElement(ElementName = "Configuration")]
        public XmlCDataSection XmlConfiguration
        {
            get
            {
                if (string.IsNullOrEmpty(this.RawContent))
                {
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(this.RawContent);
            }

            set
            {
                if (value == null)
                {
                    this.RawContent = string.Empty;
                }
                else
                {
                    this.RawContent = value.Value;
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGetResponse"/> class.
        /// </summary>
        public ConfigurationGetResponse()
        {
        }
    }
}
