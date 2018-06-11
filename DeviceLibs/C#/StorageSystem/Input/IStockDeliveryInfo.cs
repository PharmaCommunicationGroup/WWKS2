using CareFusion.Lib.StorageSystem.Stock;

namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Interface which defines the methods and properties of a storage system stock delivery information set.
    /// </summary>
    public interface IStockDeliveryInfo
    {
        #region Properties

        /// <summary>
        /// Gets the delivery number of the stock delivery.
        /// </summary>
        string DeliveryNumber { get; }

        /// <summary>
        /// Gets the current state of the stock delivery.
        /// </summary>
        StockDeliveryState State { get; }
        
        /// <summary>
        /// Gets the information of the articles and the packs that were already
        /// stored in the storage system during the processing of this stock delivery.
        /// </summary>
        IArticle[] InputArticles { get; }

        #endregion
    }
}
