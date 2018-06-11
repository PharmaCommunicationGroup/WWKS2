using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 InitiateInputResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class InitiateInputResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public InitiateInputResponse InitiateInputResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 InitiateInputResponse message.
    /// </summary>
    public class InitiateInputResponse : MessageBase
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string IsNewDelivery { get; set; }

        [XmlAttribute]
        public string SetPickingIndicator { get; set; }

        [XmlElement]
        public Details Details { get; set; }

        [XmlElement]
        public Article[] Article { get; set; }

        #endregion
    }

}
