using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Task
{
    /// <summary>
    /// Class which represents the WWKS 2.0 TaskCancelResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class TaskCancelResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public TaskCancelResponse TaskCancelResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 TaskCancelResponse message.
    /// </summary>
    public class TaskCancelResponse : MessageBase
    {
        #region Properties

        [XmlElement]
        public Types.Task[] Task { get; set; }

        #endregion
    }
}
