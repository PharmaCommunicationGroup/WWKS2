using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Handling datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Handling
    {
        #region Constants

        /// <summary>
        /// Defines the input value for the case input completed successfully.
        /// </summary>
        public const string InputCompleted = "Completed";

        /// <summary>
        /// Defines the input value for the case input has been aborted.
        /// </summary>
        public const string InputAborted = "Aborted";

        #endregion

        #region Properties

        [XmlAttribute]
        public string Input { get; set; }

        [XmlAttribute]
        public string Text { get; set; }

        #endregion
    }
}
