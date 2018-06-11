using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Enum which defines the different types result values for a set operation.
    /// </summary>
    public enum SetResultValue
    {
        /// <summary>
        /// The set operation finished successfully.
        /// </summary>
        Accepted,

        /// <summary>
        /// The set operation was rejected (mostly due to invalid parameters).
        /// </summary>
        Rejected
    }

    /// <summary>
    /// Class which represents the SetResult datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class SetResult
    {
        #region Properties

        [XmlAttribute]
        public SetResultValue Value { get; set; }

        [XmlAttribute]
        public string Text { get; set; }

        #endregion
    }
}
