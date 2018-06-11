using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.State;
using CareFusion.Lib.StorageSystem.Stock;

namespace CareFusion.Lib.StorageSystem
{
    /// <summary>
    /// Callback definition for an event which is raised whenever the state of a storage system changes.
    /// </summary>
    /// <param name="sender">Object instance which raised the event.</param>
    /// <param name="state">New state of the storage system.</param>
    public delegate void StateChangedEventHandler(IStorageSystem sender, ComponentState state);

    /// <summary>
    /// Callback definition for an event which is raised whenever the state of a storage system component changes.
    /// </summary>
    /// <param name="sender">Object instance which raised the event.</param>
    /// <param name="component">The component that has changed its state.</param>
    public delegate void ComponentStateChangedEventHandler(IStorageSystem sender, IComponent component);

    /// <summary>
    /// Callback definition for an event which is raised whenever a pack was dispensed by an output
    /// process that was not initiated by this storage system connection (e.g. at the UI of the storage system).
    /// </summary>
    /// <param name="sender">Object instance which raised the event.</param>
    /// <param name="articleList">List of articles with the packs that were dispensed.</param>
    public delegate void PackDispensedEventHandler(IStorageSystem sender, IArticle[] packList);

    /// <summary>
    /// Callback definition for an event which is raised whenever a connected storage system requests
    /// permission for pack input. The specified request object is used to get further details 
    /// and to allow or deny the pack input.
    /// </summary>
    /// <param name="sender">Object instance which raised the event.</param>
    /// <param name="request">Object which contains the details about the requested pack input.</param>
    public delegate void PackInputRequestEventHandler(IStorageSystem sender, IInputRequest request);

    /// <summary>
    /// Callback definition for an event which is raised whenever a new pack was successfully stored in a storage system. 
    /// </summary>
    /// <param name="sender">Object instance which raised the event.</param>
    /// <param name="articleList">List of articles with the packs that were stored.</param>
    public delegate void PackStoredEventHandler(IStorageSystem sender, IArticle[] articleList);

    /// <summary>
    /// Callback definition for an event which is raised whenever a pack input has been finished either successfully or not
    /// </summary>
    /// <param name="sender">Object instance which raised the event.</param>
    /// <param name="source">The identifier of the according input request source that has been finished.</param>
    /// <param name="inputRequestId">The identifier of the according input request that has been finished.</param>
    /// <param name="result">The result of the input request that has been finished.</param>
    /// <param name="articleList">List of articles with the packs that were stored.</param>
    public delegate void PackInputFinished(IStorageSystem sender, 
                                           int source, 
                                           int inputRequestId, 
                                           InputResult result,
                                           IArticle[] articleList);

    /// <summary>
    /// Callback definition for an event which is raised whenever an update of the stock occurred. 
    /// </summary>
    /// <param name="sender">Object instance which raised the event.</param>
    /// <param name="articleList">List of articles and the packs that were updated.</param>
    public delegate void StockUpdatedEventHandler(IStorageSystem sender, IArticle[] articleList);

    /// <summary>
    /// Callback definition for an event which is raised whenever an output process has finished.
    /// This event is equivalent to the Finished event of an IOutputProcess object. The main 
    /// difference is, that it is thrown for every output process that has an order number not equal to "1".
    /// Even if the output process has not been started by the storage system instance.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="orderNumber">The order number of the output process.</param>
    /// <param name="outputProcess">The output process that has finished execution.</param>
    public delegate void OutputProcessFinishedEventHandler(IStorageSystem sender, string orderNumber, IOutputProcessInfo outputProcess);
}
