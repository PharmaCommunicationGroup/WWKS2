using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 InitiateInputMessage message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class InitiateInputMessageEnvelope : EnvelopeBase
    {
        [XmlElement]
        public InitiateInputMessage InitiateInputMessage { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 InitiateInputMessage message.
    /// </summary>
    public class InitiateInputMessage : MessageBase
    {
        #region WWKS 2.0 Properties

        [XmlElement]
        public Details Details { get; set; }

        [XmlElement]
        public List<Article> Article { get; set; }

        #endregion
    }
}
