using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Tag datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Tag : ITag
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Value { get; set; }

        #endregion

        #region ITag Specific Properties

        /// <summary>
        /// Gets the value of the tag.
        /// </summary>
        [XmlIgnore]
        string ITag.Value
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.Value);
            }
        }

        #endregion
    }
}
