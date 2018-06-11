using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.KeepAlive
{
    /// <summary>
    /// Class which represents the WWKS 2.0 KeepAliveRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class KeepAliveRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public KeepAliveRequest KeepAliveRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 KeepAliveRequest message.
    /// </summary>
    public class KeepAliveRequest : MessageBase
    {
    }
}
