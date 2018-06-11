using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Task
{
    /// <summary>
    /// Class which represents the WWKS 2.0 TaskInfoRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class TaskInfoRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public TaskInfoRequest TaskInfoRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 TaskInfoRequest message.
    /// </summary>
    public class TaskInfoRequest : MessageBase
    {
        #region Properties

        [XmlAttribute]
        public string IncludeTaskDetails { get; set; }

        [XmlElement]
        public Types.Task[] Task { get; set; }

        #endregion
    }
}
