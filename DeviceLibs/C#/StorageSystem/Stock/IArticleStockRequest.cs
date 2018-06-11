
namespace CareFusion.Lib.StorageSystem.Stock
{
    /// <summary>
    /// Interface which defines the methods and properties of a storage system article.
    /// </summary>
    public interface IArticleStockRequest
    {
        /// <summary>
        /// Gets the unique identifier of the article (e.g. PZN or EAN).
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Adds a pack to this article stock.
        /// </summary>
        /// <param name="subItemQuantity">
        /// Optional number of elements (e.g. pills or ampoules) which are in one open pack of this article.
        /// A value of 0 means a full pack.
        /// </param>
        void AddPack(uint subItemQuantity = 0);

        /// <summary>
        /// Adds a multiple packs to this article stock.
        /// </summary>
        /// <param name="packCount"> Number of packs to add. </param>
        void AddMultiplePacks(int packCount);
    }
}
