using CareFusion.Lib.StorageSystem.Stock;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the stock location datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class StockLocation : IStockLocation
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        #endregion

        #region IStockLocation Properties

        /// <summary>
        /// Gets the unique identifier of the stock location.
        /// </summary>
        string IStockLocation.Id
        {
            get { return this.Id != null ? TextConverter.EscapeInvalidXmlChars(this.Id) : string.Empty; }
        }

        /// <summary>
        /// Gets the description of the stock location.
        /// </summary>
        string IStockLocation.Description
        {
            get { return this.Description != null ? TextConverter.EscapeInvalidXmlChars(this.Description) : string.Empty; }
        }

        #endregion        
    }
}
