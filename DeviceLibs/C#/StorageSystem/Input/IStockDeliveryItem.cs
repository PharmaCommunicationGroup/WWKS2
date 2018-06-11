using System;

namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Interface which defines the methods and properties of a storage system stock delivery item.
    /// </summary>
    public interface IStockDeliveryItem
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier of the article (e.g. PZN or EAN).
        /// </summary>
        string ArticleId { get; }

        /// <summary>
        /// Gets the name of the article.
        /// </summary>
        string ArticleName { get; }

        /// <summary>
        /// Gets the dosage form of the article.
        /// </summary>
        string DosageForm { get; }

        /// <summary>
        /// Gets the packaging unit of the article.
        /// </summary>
        string PackagingUnit { get; }

        /// <summary>
        /// Gets a flag whether packs of this article have to be stored in a refrigerator.
        /// </summary>
        bool RequiresFridge { get; }

        /// <summary>
        /// Gets the optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means - "no maximum defined".
        /// </summary>
        uint MaxSubItemQuantity { get; }

        /// <summary>
        /// Gets the batch number which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with the according stock delivery.
        /// </summary>
        string BatchNumber { get; }

        /// <summary>
        /// Gets the external identifier which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with the according stock delivery.
        /// </summary>
        string ExternalId { get; }

        /// <summary>
        /// Gets the expiry date which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with the according stock delivery.
        /// If this property is null, the storage system will automatically choose an expiry date.
        /// </summary>
        Nullable<DateTime> ExpiryDate { get; }

        /// <summary>
        /// Optional limitation of how many packs of this article are allowed 
        /// to be stored in the storage system in combination with the according stock delivery.
        /// </summary>
        uint MaxAllowedPackCount { get; }

        /// <summary>
        /// Gets the stock location which is allowed to use for packs of this article.
        /// </summary>
        string StockLocationId { get; }

        /// <summary>
        /// Gets the machine location which is allowed to use for packs of this article.
        /// </summary>
        string MachineLocation { get; }

        #endregion
    }
}
