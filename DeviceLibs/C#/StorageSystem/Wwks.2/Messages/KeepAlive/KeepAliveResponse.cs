using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.KeepAlive
{
    /// <summary>
    /// Class which represents the WWKS 2.0 KeepAliveResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class KeepAliveResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public KeepAliveResponse KeepAliveResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 KeepAliveResponse message.
    /// </summary>
    public class KeepAliveResponse : MessageBase
    {
    }
}
