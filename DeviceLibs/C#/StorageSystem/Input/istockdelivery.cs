using System;

namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Interface which defines the methods and properties of a storage system stock delivery.
    /// </summary>
    public interface IStockDelivery
    {
        #region Properties

        /// <summary>
        /// Gets the delivery number of the stock delivery.
        /// </summary>
        string DeliveryNumber { get; }

        /// <summary>
        /// Gets the list of defined stock delivery items.
        /// </summary>
        IStockDeliveryItem[] Items { get; }

        #endregion

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
        void AddItem(string articleId, 
                     string articleName,
                     string packagingUnit,
                     string dosageForm,                     
                     bool requiresFridge = false,
                     uint maxSubItemQuantity = 0,
                     string batchNumber = null,
                     string externalId = null,
                     Nullable<DateTime> expiryDate = null,
                     uint maxAllowedPackCount = 0,
                     string stockLocationId = null,
                     string machineLocation = null);
    }
}
