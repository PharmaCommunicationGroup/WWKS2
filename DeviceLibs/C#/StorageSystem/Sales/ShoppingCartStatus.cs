namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Defines the possible states of a shopping cart.
    /// </summary>
    public enum ShoppingCartStatus
    {
        /// <summary>
        /// The shopping cart is active and can still be manipulated.
        /// </summary>
        Active,

        /// <summary>
        /// The shopping cart has been closed and can't be manipulated anymore.
        /// </summary>
        Finished,

        /// <summary>
        /// The shopping cart has been discarded.
        /// </summary>
        Discarded
    }
}
