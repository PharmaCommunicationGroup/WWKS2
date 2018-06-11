using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Configuration
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ConfigurationGetRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ConfigurationGetRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ConfigurationGetRequest ConfigurationGetRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ConfigurationGetRequest message.
    /// </summary>
    public class ConfigurationGetRequest : MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGetRequest"/> class.
        /// </summary>
        public ConfigurationGetRequest()
        {
        }
    }
}
