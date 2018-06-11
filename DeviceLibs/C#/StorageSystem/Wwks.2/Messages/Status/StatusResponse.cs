using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Status
{
    /// <summary>
    /// Class which represents the WWKS 2.0 StatusResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class StatusResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public StatusResponse StatusResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 StatusResponse message.
    /// </summary>
    public class StatusResponse : MessageBase
    {
        #region Properties

        [XmlAttribute]
        public string State { get; set; }

        [XmlAttribute]
        public string StateText { get; set; }

        [XmlElement]
        public Component[] Component { get; set; }

        #endregion
    }
}
