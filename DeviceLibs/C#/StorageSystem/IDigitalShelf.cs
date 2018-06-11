using CareFusion.Lib.StorageSystem.Sales;
using System;

namespace CareFusion.Lib.StorageSystem
{
    /// <summary>
    /// Interface which defines the methods, properties and events of a digital shelf connection.
    /// Classes implementing this interface provide It-Systems with the capability to connect to and interact with a digital shelf system (e.g. Rowa VMotion).
    /// </summary>
    public interface IDigitalShelf : IDisposable
    {
		#region Properties
		
		 /// <summary>
       /// Gets a flag whether the digital shelf is currently connected.
       /// </summary>
       bool Connected { get; }

		/// <summary>
		/// Gets the subscriber id of the digital shelf.
		/// </summary>
		int SubscriberID{ get; }

		#endregion

        #region Events

        /// <summary>
        /// Event which is raised when an article has been selected on the screen of the digital shelf.
        /// </summary>
        event ArticleSelectedEventHandler ArticleSelected;

        /// <summary>
        /// Event which is raised when a digital shelf requests detailed information for one or more articles.
        /// </summary>
        event ArticleInfoRequestEventHandler ArticleInfoRequested;

        /// <summary>
        /// Event which is raised when a digital shelf requests price information for one or more articles.
        /// </summary>
        event ArticlePriceRequestEventHandler ArticlePriceRequested;

        /// <summary>
        /// Event which is raised when a digital shelf requests a shopping cart.
        /// This can either be a new, empty shopping cart or an existing one (depending on the critiera specified).
        /// </summary>
        event ShoppingCartRequestEventHandler ShoppingCartRequested;

        /// <summary>
        /// Event which is raised when a shopping cart has been manipulated on a digital shelf.
        /// </summary>
        event ShoppingCartUpdateRequestEventHandler ShoppingCartUpdateRequested;

        /// <summary>
        /// Event which is raised when a digital shelf need to know some stock
        /// </summary>
        event StockInfoRequestEventHandler StockInfoRequested;

        #endregion

        #region Methods

        /// <summary>
        /// Establishes a new connection to the digital shelf with the the specified host at the specified port.
        /// This method performs an implicit disconnect if there is already an active digital shelf connection.
        /// </summary>
        /// <param name="host">The name or ip address of the digital shelf.</param>
        /// <param name="port">The port number of the digital shelf. Default is 6052.</param>
        void Connect(string host, ushort port = 6052);

        /// <summary>
        /// Performs a graceful shutdown of the digital shelf connection. 
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Creates a new shopping cart instance.
        /// </summary>
        /// <param name="id">The unique id of the shopping cart.</param>
        /// <param name="status">The status of the shopping cart.</param>
        /// <param name="customerId">The unique id of the customer the shopping cart belongs to.</param>
        /// <param name="salesPersonId">The unique id of the corresponding sales person.</param>
        /// <param name="salesPointId">The unique id of the corresponding sales point.</param>
        /// <param name="viewPointId">The unique id of the corresponding view point.</param>
        /// <returns>An <see cref="IShoppingCart"/> instance with the specified attributes.</returns>
        IShoppingCart CreateShoppingCart(string id, ShoppingCartStatus status, string customerId = "", string salesPersonId = "", string salesPointId = "", string viewPointId = "");

        /// <summary>
        /// Notifies the connected digital shelf/shelves when a shopping cart has been manipulated in the It-System.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart that has been manipulated.</param>
        /// <returns><c>true</c> if the shopping cart update message was sent successfully, <c>false</c> otherwise.</returns>
        bool UpdateShoppingCart(IShoppingCart shoppingCart);

        #endregion
    }
}
