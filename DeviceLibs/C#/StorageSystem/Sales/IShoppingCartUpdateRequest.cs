namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of a shopping cart update request.
    /// </summary>
    public interface IShoppingCartUpdateRequest
    {
        /// <summary>
        /// Gets the shopping cart which is to be updated.
        /// </summary>
        IShoppingCart ShoppingCart { get; }

        /// <summary>
        /// Accepts the update request after the shopping cart has been updated in the It-System.
        /// </summary>
        /// <param name="description">An optional message to be sent with the response.</param>
        void Accept(string description);

        /// <summary>
        /// Rejects the update request without updating the shopping cart in the It-System.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart in its current, not updated status.</param>
        /// <param name="description">An optional message to be sent with the response(e.g. the reason for rejecting the request).</param>
        void Reject(IShoppingCart shoppingCart, string description);
    }
}
