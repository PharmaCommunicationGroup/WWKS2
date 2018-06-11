namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of a shopping cart criteria.
    /// </summary>
    public interface IShoppingCartCriteria
    {
        #region Properties

        /// <summary>
        /// Gets the unique Id of the shopping cart.
        /// </summary>
        string ShoppingCartId { get; }

        /// <summary>
        /// Gets the unique Id of the sales point the shopping cart is currently assigned to.
        /// </summary>
        string SalesPointId { get; }

        /// <summary>
        /// Gets the unique Id of the view point the shopping cart is currently being processed on.
        /// </summary>
        string ViewPointId { get; }

        /// <summary>
        /// Gets the unique Id of the sales person that the shopping cart is currently assigned to.
        /// </summary>
        string SalesPersonId { get; }

        /// <summary>
        /// Gets the unique Id of the customer who the shopping cart belongs to.
        /// </summary>
        string CustomerId { get; }

        #endregion
    }
}
