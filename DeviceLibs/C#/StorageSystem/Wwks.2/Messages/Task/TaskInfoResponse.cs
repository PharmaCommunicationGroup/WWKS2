using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Task
{
    /// <summary>
    /// Class which represents the WWKS 2.0 TaskInfoResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class TaskInfoResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public TaskInfoResponse TaskInfoResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 TaskInfoResponse message.
    /// </summary>
    public class TaskInfoResponse : MessageBase
    {
        #region Properties

        [XmlElement]
        public Types.Task[] Task { get; set; }

        #endregion
    }
}
