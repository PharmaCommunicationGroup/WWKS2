using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 InputMessage message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class InputMessageEnvelope : EnvelopeBase
    {
        [XmlElement]
        public InputMessage InputMessage { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 InputMessage message.
    /// </summary>
    public class InputMessage : MessageBase
    {
        #region Properties

        [XmlAttribute]
        public string IsNewDelivery { get; set; }

        [XmlElement]
        public List<Article> Article { get; set; }

        #endregion
    }
}
