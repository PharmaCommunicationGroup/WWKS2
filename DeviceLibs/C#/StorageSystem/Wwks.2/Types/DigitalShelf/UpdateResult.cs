using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf
{
    /// <summary>
    /// Defines the possible results of a shopping cart update request.
    /// </summary>
    public enum UpdateResultStatus
    {
        /// <summary>
        /// The shopping cart has been updated successfully.
        /// </summary>
        Updated,

        /// <summary>
        /// The shopping cart has not been updated.
        /// </summary>
        NotUpdated
    }

    /// <summary>
    /// Class which represents the UpdateResult datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class UpdateResult
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public UpdateResultStatus Status { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        #endregion
    }
}
