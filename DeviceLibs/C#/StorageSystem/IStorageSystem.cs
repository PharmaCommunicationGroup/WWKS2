using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.State;
using CareFusion.Lib.StorageSystem.Stock;
using System;
using System.Collections.Generic;

namespace CareFusion.Lib.StorageSystem
{
    /// <summary>
    /// Interface which defines the methods and properties of a storage system connection.
    /// </summary>
    public interface IStorageSystem : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets a flag whether the storage system is currently connected.
        /// </summary>
        bool Connected { get; }
		
			/// <summary>
			/// Gets the subscriber id of the storage system.
			/// </summary>
			int SubscriberID{ get; }

        /// <summary>
        /// Gets the current state of the storage system.
        /// </summary>
        ComponentState State { get; }

        /// <summary>
        /// Gets a list of all components of the storage system with their according states.
        /// </summary>
        IComponent[] ComponentStates { get; }

        /// <summary>
        /// Gets the arbitrary configuration content of the storage system.
        /// </summary>
        string Configuration { get; }

        /// <summary>
        /// Gets a list of all stock locations that are supported by the storage system.
        /// </summary>
        IStockLocation[] StockLocations { get; }

        /// <summary>
        /// Enables or disables the automatic state observation. 
        /// If it is enabled, the state of the storage system is requested regularly
        /// and in case of state changes the event "StateChanged" is raised.
        /// Automatic state observation is enabled by default.
        /// </summary>
        bool EnableAutomaticStateObservation { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event which is raised when the state of a storage system has changed.
        /// </summary>
        event StateChangedEventHandler StateChanged;

        /// <summary>
        /// Event which is raised when the state of a storage system component has changed.
        /// </summary>
        event ComponentStateChangedEventHandler ComponentStateChanged;

        /// <summary>
        /// Event which is raised when a pack was dispensed by an output
        /// process that was not initiated by this storage system connection (e.g. at the UI of the storage system).
        /// </summary>
        event PackDispensedEventHandler PackDispensed;

        /// <summary>
        /// Event which is raised whenever a connected storage system requests permission for pack input.
        /// </summary>
        event PackInputRequestEventHandler PackInputRequested;

        /// <summary>
        /// Event which is raised whenever a connected storage system finished an input.
        /// </summary>
        event PackInputFinished PackInputFinished;

        /// <summary>
        /// Event which is raised whenever a new pack was successfully stored in a storage system. 
        /// </summary>
        event PackStoredEventHandler PackStored;

        /// <summary>
        /// Event which is raised whenever the stock of the storage system has been updated.
        /// </summary>
        event StockUpdatedEventHandler StockUpdated;

        /// <summary>
        /// Event which is raised whenever an output process finished processing.
        /// </summary>
        event OutputProcessFinishedEventHandler OutputProcessFinished;

        #endregion

        #region Methods

        /// <summary>
        /// Establishes a new connection to the storage system with the the specified host at the specified port.
        /// This method performs an implicit disconnect if there is already an active storage system connection.
        /// </summary>
        /// <param name="host">The name or ip address of the storage system.</param>
        /// <param name="port">The port number of the storage system. Default is 6050.</param>
        void Connect(string host, ushort port = 6050);
        
        /// <summary>
        /// Performs a graceful shutdown of the storage system connection. 
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets the complete stock of the connected storage system.
        /// </summary>
        /// <param name="includePackDetails">
        /// Flag whether the returned article list should also include detailed pack information.
        /// </param>
        /// <param name="includeArticleDetails">
        /// Flag whether the returned article list should also contain detailed article information 
        /// like name, dosage form, packaging unit, etc.
        /// </param>
        /// <returns>
        /// List of articles and packs which are currently stored in the storage system.
        /// </returns>
        List<IArticle> GetStock(bool includePackDetails = true, bool includeArticleDetails = false);

        /// <summary>
        /// Gets the stock which matches the specified filter criteria from the connected storage system.
        /// </summary>
        /// <param name="includePackDetails">
        /// Flag whether the returned article list should also include detailed pack information.
        /// </param>
        /// <param name="includeArticleDetails">
        /// Flag whether the returned article list should also contain detailed article information 
        /// like name, dosage form, packaging unit, etc.
        /// </param>
        /// <param name="articleId">
        /// If this parameter is not null, the requested stock information is filtered by the specified article identifier.
        /// </param>
        /// <param name="batchNumber">
        /// If this parameter is not null, the requested stock information is filtered by the specified batch number.
        /// </param>
        /// <param name="externalId">
        /// If this parameter is not null, the requested stock information is filtered by the specified external pack identifier.
        /// </param>
        /// <param name="stockLocationId">
        /// If this parameter is not null, the requested stock information is filtered by the specified stock location.
        /// </param>
        /// <param name="machineLocation">
        /// If this parameter is not null, the requested stock information is filtered by the specified machine location.
        /// </param> 
        /// <returns>
        /// List of articles and packs which are currently stored in the storage system and matches the specified filter.
        /// </returns>
        List<IArticle> GetStock(bool includePackDetails, 
                                bool includeArticleDetails,
                                string articleId, 
                                string batchNumber = null, 
                                string externalId = null,
                                string stockLocationId = null,
                                string machineLocation = null);

        /// <summary>
        /// Creates a new object instance of a master article.
        /// Master articles are used by the storage system to make self-sufficient decisions 
        /// about which articles are allowed to be stored in the storage system. If the storage
        /// system is configured to use its master article list, it will not send InputRequest
        /// for every scanned pack. Instead it will decide on its own whether to input a pack.
        /// </summary>
        /// <param name="articleId">The unique identifier of the article (e.g. PZN or EAN).</param>
        /// <param name="articleName">The name of the article.</param>
        /// <param name="dosageForm">The dosage form of the article.</param>
        /// <param name="packagingUnit">The packaging unit of the article.</param>
        /// <param name="requiresFridge">The flag whether packs of this article have to be stored in a refrigerator.</param>
        /// <param name="maxSubItemQuantity">
        /// The optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        /// <param name="stockLocationId">Optional stock location to use for packs of this article.</param>
        /// <param name="machineLocation">Optional machine location to use for packs of this article.</param> 
        /// <returns>Newly created master article object instance.</returns>
        IMasterArticle CreateMasterArticle(string articleId, 
                                           string articleName, 
                                           string dosageForm, 
                                           string packagingUnit, 
                                           bool requiresFridge = false, 
                                           uint maxSubItemQuantity = 0,
                                           string stockLocationId = null,
                                           string machineLocation = null);

        /// <summary>
        /// Updates the list of active master articles which are used by the storage system.
        /// </summary>
        /// <param name="masterArticleList">
        /// The new list of master articles to set. 
        /// If the list is empty, the master article list of the storage system is cleared.
        /// </param>
        void UpdateMasterArticles(List<IMasterArticle> masterArticleList);

        /// <summary>
        /// Creates a new object instance of a stock delivery.
        /// Pre-defined stock deliveries are used by the storage system to make self-sufficient decisions 
        /// about which stock deliveries are allowed to be started at the storage system and which
        /// articles are allowed to be stored in combination with a specific stock delivery. 
        /// If the storage system is configured to use pre-defined stock deliveries, it will not 
        /// send InputRequest for every scanned pack that belongs to a stock delivery. 
        /// Instead it will decide on its own whether to input a pack.
        /// </summary>
        /// <param name="deliveryNumber">The delivery number of the stock delivery.</param>
        /// <returns>Newly created stock delivery object instance.</returns>
        IStockDelivery CreateStockDelivery(string deliveryNumber);

        /// <summary>
        /// Adds the specified list of stock deliveries to the pre-defined stock delivery list of the storage system.
        /// </summary>
        /// <param name="stockDeliveryList">List of stock deliveries to add.</param>
        void AddStockDeliveries(List<IStockDelivery> stockDeliveryList);

        /// <summary>
        /// Gets detailed information about the specified stock delivery.
        /// </summary>
        /// <param name="deliveryNumber">The delivery number of the stock delivery.</param>
        /// <returns>According stock delivery information.</returns>
        IStockDeliveryInfo GetStockDeliveryInfo(string deliveryNumber);

        /// <summary>
        /// Creates a new output process which can be used to dispense packs from the connected storage system.
        /// </summary>
        /// <param name="orderNumber">
        /// The unique order number which is used to identify an output process.
        /// </param>
        /// <param name="outputDestination">
        /// The storage system output destination for the dispensed packs.
        /// This number typically refers to a dispense point which has been configured at the storage system
        /// (e.g. 1 = maintenance output).
        /// </param>
        /// <param name="outputPoint">
        /// A more detailed definition of the storage system output destination (e.g. belt number) for the dispensed packs.
        /// </param>
        /// <param name="priority">
        /// The priority of this output process.
        /// </param>
        /// <param name="boxNumber">
        /// The optional box number which belongs to this output process.
        /// This parameter is required for box system scenarios when the IT system
        /// wants to dispense packs to a specific box.
        /// </param>
        /// <returns>Newly created output process to use for dispensing packs.</returns>
        IOutputProcess CreateOutputProcess(int orderNumber, 
                                           int outputDestination, 
                                           int outputPoint = 0,
                                           OutputProcessPriority priority = OutputProcessPriority.Normal, 
                                           string boxNumber = null);

        /// <summary>
        /// Creates a new output process which can be used to dispense packs from the connected storage system.
        /// </summary>
        /// <param name="orderNumber">
        /// The unique order number which is used to identify an output process.
        /// </param>
        /// <param name="outputDestination">
        /// The storage system output destination for the dispensed packs.
        /// This number typically refers to a dispense point which has been configured at the storage system
        /// (e.g. 1 = maintenance output).
        /// </param>
        /// <param name="outputPoint">
        /// A more detailed definition of the storage system output destination (e.g. belt number) for the dispensed packs.
        /// </param>
        /// <param name="priority">
        /// The priority of this output process.
        /// </param>
        /// <param name="boxNumber">
        /// The optional box number which belongs to this output process.
        /// This parameter is required for box system scenarios when the IT system
        /// wants to dispense packs to a specific box.
        /// </param>
        /// <returns>Newly created output process to use for dispensing packs.</returns>
        IOutputProcess CreateOutputProcess(string orderNumber,
                                           int outputDestination,
                                           int outputPoint = 0,
                                           OutputProcessPriority priority = OutputProcessPriority.Normal,
                                           string boxNumber = null);

        /// <summary>
        /// Gets detailed information about the specified output process.
        /// </summary>
        /// <param name="orderNumber">The unique order number which identifies the output process.</param>
        /// <returns>According output process information.</returns>
        IOutputProcessInfo GetOutputProcessInfo(int orderNumber);

        /// <summary>
        /// Gets detailed information about the specified output process.
        /// </summary>
        /// <param name="orderNumber">The unique order number which identifies the output process.</param>
        /// <returns>According output process information.</returns>
        IOutputProcessInfo GetOutputProcessInfo(string orderNumber);

        /// <summary>
        /// Creates a new input initiation request which can be used to trigger an input request of the connected storage system.
        /// </summary>
        /// <param name="id">The unique identifier of the initiated input process.</param>
        /// <param name="inputSource">The storage system input source for the pack input.</param>
        /// <param name="inputPoint">The optional input point (e.g. belt number) to use for pack input.</param>
        /// <param name="destination">The destination identifier of the system that should start the input process.</param>
        /// <param name="deliveryNumber">Delivery number to use when the initiated input is part of a new delivery.
        /// A value of null means that the input is part of a stock return.</param>
        /// <param name="setPickingIndicator">Flag whether the initiated input should request the activation of the picking indicator.</param>
        /// <returns>
        /// Newly created input initiation request if initiation of inputs is supported;null otherwise.
        /// </returns>
        IInitiateInputRequest CreateInitiateInputRequest(int id,
                                                         int inputSource,
                                                         int inputPoint = 0,
                                                         int destination = 0,
                                                         string deliveryNumber = null,
                                                         bool setPickingIndicator = false);

        #endregion
    }
}
