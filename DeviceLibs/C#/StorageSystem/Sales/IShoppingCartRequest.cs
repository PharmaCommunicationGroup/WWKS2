namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of a shopping cart request.
    /// </summary>
    public interface IShoppingCartRequest
    {
        /// <summary>
        /// Gets the criteria for the shopping cart.
        /// </summary>
        IShoppingCartCriteria Criteria { get; }

        /// <summary>
        /// Accepts the shopping cart request and sends the corresponding shopping cart response.
        /// </summary>
        /// <param name="shoppingCart">The requested shopping cart.</param>
        void Accept(IShoppingCart shoppingCart);

        /// <summary>
        /// Rejects the udpate request.
        /// No shopping cart will be sent to the digital shelf.
        /// </summary>
        void Reject();
    }
}
