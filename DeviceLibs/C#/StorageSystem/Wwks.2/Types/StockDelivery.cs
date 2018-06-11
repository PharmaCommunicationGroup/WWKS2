using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the StockDelivery datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class StockDelivery : IStockDelivery
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string DeliveryNumber { get; set; }

        [XmlElement]
        public List<Article> Article { get; set; }

        #endregion
        
        #region IStockDelivery Specific Properties

        /// <summary>
        /// Gets the delivery number of the stock delivery.
        /// </summary>
        [XmlIgnore]
        string IStockDelivery.DeliveryNumber { get { return TextConverter.UnescapeInvalidXmlChars(this.DeliveryNumber); } }

        /// <summary>
        /// Gets the list of defined stock delivery items.
        /// </summary>
        [XmlIgnore]
        IStockDeliveryItem[] IStockDelivery.Items
        {
            get 
            { 
                return (this.Article != null) ? (IStockDeliveryItem[])Article.ToArray() : new IStockDeliveryItem[0]; 
            }
        }

        #endregion

        #region IStockDelivery Specific Methods

        /// <summary>
        /// Adds a new delivery item to this stock delivery.
        /// </summary>
        /// <param name="articleId">
        /// Unique identifier of the article (e.g. PZN or EAN).
        /// </param>
        /// <param name="articleName">
        /// Name of the article.
        /// </param>
        /// <param name="dosageForm">
        /// Dosage form of the article.
        /// </param>
        /// <param name="packagingUnit">
        /// Packaging unit of the article.
        /// </param>
        /// <param name="requiresFridge">
        /// Flag whether packs of this article have to be stored in a refrigerator.
        /// </param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        /// <param name="batchNumber">
        /// Optional batch number which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with this stock delivery.
        /// </param>
        /// <param name="externalId">
        /// Optional external identifier which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with this stock delivery.
        /// </param>
        /// <param name="expiryDate">
        /// Optional expiry date which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with this stock delivery.
        /// If this value is null, the storage system will automatically choose an expiry date.
        /// </param>
        /// <param name="maxAllowedPackCount">
        /// Optional limitation of how many packs of this article are allowed 
        /// to be stored in the storage system in combination with this stock delivery.
        /// </param>
        /// <param name="stockLocationId">Optional stock location to use for the packs of this article.</param>
        /// <param name="machineLocation">Optional machine location to use for the packs of this article.</param>
        void IStockDelivery.AddItem(string articleId, 
                                    string articleName, 
                                    string packagingUnit, 
                                    string dosageForm, 
                                    bool requiresFridge, 
                                    uint maxSubItemQuantity, 
                                    string batchNumber, 
                                    string externalId, 
                                    DateTime? expiryDate, 
                                    uint maxAllowedPackCount,
                                    string stockLocationId,
                                    string machineLocation)
        {
            if (string.IsNullOrEmpty(articleId))
            {
                throw new ArgumentException("Invalid articleId specified.");
            }

            if (this.Article == null)
            {
                this.Article = new List<Article>();
            }

            this.Article.Add(new Article() 
            { 
                Id = TextConverter.EscapeInvalidXmlChars(articleId),
                Name = TextConverter.EscapeInvalidXmlChars(articleName),
                PackagingUnit = TextConverter.EscapeInvalidXmlChars(packagingUnit),
                DosageForm = TextConverter.EscapeInvalidXmlChars(dosageForm),
                RequiresFridge = requiresFridge.ToString(),
                MaxSubItemQuantity = maxSubItemQuantity.ToString(),
                BatchNumber = TextConverter.EscapeInvalidXmlChars(batchNumber),
                ExternalId = TextConverter.EscapeInvalidXmlChars(externalId),
                ExpiryDate = expiryDate.HasValue ? TypeConverter.ConvertDate(expiryDate.Value) : null,
                Quantity = (maxAllowedPackCount > 0) ? maxAllowedPackCount.ToString() : null,
                StockLocationId = TextConverter.EscapeInvalidXmlChars(stockLocationId),
                MachineLocation = TextConverter.EscapeInvalidXmlChars(machineLocation)
            });
        }

        #endregion
    }
}
