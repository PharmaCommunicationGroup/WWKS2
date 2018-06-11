using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Output
{
    /// <summary>
    /// Class which represents the WWKS 2.0 OutputResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class OutputResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public OutputResponse OutputResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 OutputResponse message.
    /// </summary>
    public class OutputResponse : MessageBase
    {
        #region Properties

        [XmlElement]
        public Details Details { get; set; }

        [XmlElement]
        public Criteria[] Criteria { get; set; }

        [XmlAttribute]
        public string BoxNumber { get; set; }

        #endregion
    }
}
