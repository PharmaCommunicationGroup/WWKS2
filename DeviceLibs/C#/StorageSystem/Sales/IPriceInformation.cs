namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of an article price information.
    /// </summary>
    public interface IPriceInformation
    {
        /// <summary>
        /// Gets the category of the price information (e.g. "RRP" or "Offer").
        /// </summary>
        PriceCategory Category { get; }

        /// <summary>
        /// Gets the description of the price information (e.g. the name of a special offer).
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the quantity for which the price is valid (e.g "5" if this price is valid if the customer buys at least 5 packs of the article).
        /// </summary>
        uint Quantity { get; }

        /// <summary>
        /// Gets the actual price specified in the smallest unit of the specified currency (e.g. Cent).
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// Gets the base price specified in the smallest unit of the specified currency (e.g. Cent).
        /// </summary>
        decimal BasePrice { get; }

        /// <summary>
        /// Gets the base price unit for the specified base price (e.g. "100ml").
        /// </summary>
        string BasePriceUnit { get; }
        
        /// <summary>
        /// Gets the VAT for the specified base price.
        /// </summary>
        string VAT { get; }
    }
}
