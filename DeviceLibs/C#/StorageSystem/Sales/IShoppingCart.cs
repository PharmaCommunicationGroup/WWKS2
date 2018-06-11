namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of a shopping cart.
    /// </summary>
    public interface IShoppingCart
    {
        #region Properties

        /// <summary>
        /// Gets the unique Id of the shopping cart.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the current status of the shopping cart.
        /// </summary>
        ShoppingCartStatus Status { get; set; }

        /// <summary>
        /// Gets the unique Id of the sales point the shopping cart is currently assigned to.
        /// </summary>
        string SalesPointId { get; set; }

        /// <summary>
        /// Gets the unique Id of the view point the shopping cart is currently being processed on.
        /// </summary>
        string ViewPointId { get; set; }

        /// <summary>
        /// Gets the unique Id of the sales person that the shopping cart is currently assigned to.
        /// </summary>
        string SalesPersonId { get; set; }

        /// <summary>
        /// Gets the unique Id of the customer who the shopping cart belongs to.
        /// </summary>
        string CustomerId { get; set; }

        /// <summary>
        /// Gets the detailed information of the items the shopping cart currently contains.
        /// </summary>
        IShoppingCartItem[] ShoppingCartItems { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to the shopping cart.
        /// </summary>
        /// <param name="articleId">The unique article Id of the shopping cart item.</param>
        /// <param name="orderedQuantity">The total amount of packs of this article in the shopping cart.</param>
        /// <param name="dispensedQuantity">The amount of packs that has already been dispensed from a storage system.</param>
        /// <param name="paidQuantity">The amount of packs that have been paid.</param>
        /// <param name="price">The total price specified in the smallest unit of the currency (e.g. Cent).</param>
        /// <param name="currency">The ISO 4217 currency symbol in which the price is specified (e.g. EUR).</param>
        void AddItem(string articleId, uint orderedQuantity, uint dispensedQuantity = 0, uint paidQuantity = 0, string price = "", string currency = "");

        #endregion
    }
}
