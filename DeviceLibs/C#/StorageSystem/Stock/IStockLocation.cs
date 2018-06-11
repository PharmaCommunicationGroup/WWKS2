
namespace CareFusion.Lib.StorageSystem.Stock
{
    /// <summary>
    /// Interface which defines the methods and properties of a stock location.
    /// </summary>
    public interface IStockLocation
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier of the stock location.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the description of the stock location.
        /// </summary>
        string Description { get; }

        #endregion
    }
}
