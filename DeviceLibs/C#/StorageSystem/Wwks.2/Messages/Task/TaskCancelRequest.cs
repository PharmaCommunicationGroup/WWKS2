using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Task
{
    /// <summary>
    /// Class which represents the WWKS 2.0 TaskCancelRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class TaskCancelRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public TaskCancelRequest TaskCancelRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 TaskCancelRequest message.
    /// </summary>
    public class TaskCancelRequest : MessageBase
    {
        #region Properties

        [XmlElement]
        public Types.Task[] Task { get; set; }

        #endregion
    }
}
