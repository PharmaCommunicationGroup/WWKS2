using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Status
{
    /// <summary>
    /// Class which represents the WWKS 2.0 StatusRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class StatusRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public StatusRequest StatusRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 StatusRequest message.
    /// </summary>
    public class StatusRequest : MessageBase
    {
        #region Properties

        [XmlAttribute]
        public bool IncludeDetails { get; set; }

        #endregion

        public StatusRequest()
        {
            this.IncludeDetails = false;
        }
    }
}
