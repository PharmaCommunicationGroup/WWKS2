namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of a shopping cart item.
    /// </summary>
    public interface IShoppingCartItem
    {
        /// <summary>
        /// Gets the unique article Id of the shopping cart item.
        /// </summary>
        string ArticleId { get; }

        /// <summary>
        /// Gets the total amount of packs of this article in the shopping cart.
        /// </summary>
        uint OrderedQuantity { get; }

        /// <summary>
        /// Gets the amount of packs that has already been dispensed from a storage system.
        /// </summary>
        uint DispensedQuantity { get; }

        /// <summary>
        /// Gets the amount of packs that have been paid.
        /// </summary>
        uint PaidQuantity { get; }

        /// <summary>
        /// Gets the total price specified in the smallest unit of the currency (e.g. Cent).
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// Gets the ISO 4217 currency symbol in which the price is specified (e.g. EUR).
        /// </summary>
        string Currency { get; }
    }
}
