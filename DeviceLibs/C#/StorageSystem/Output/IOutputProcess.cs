using System;

namespace CareFusion.Lib.StorageSystem.Output
{
    /// <summary>
    /// Interface which defines the methods and properties of a pack output process.
    /// </summary>
    public interface IOutputProcess : IOutputProcessInfo
    {
        #region Properties

        /// <summary>
        /// Gets the unique order number which is used to identify an output process.
        /// </summary>
        string OrderNumber { get; }

        /// <summary>
        /// Gets the priority of the output process.
        /// </summary>
        OutputProcessPriority Priority { get; }

        /// <summary>
        /// Gets the output destination identifier for the output process.
        /// </summary>
        int OutputDestination { get; }

        /// <summary>
        /// Gets the more detailed output destination point (e.g. belt number) for the output process.
        /// </summary>
        int OutputPoint { get; }

        /// <summary>
        /// Gets the optional box number which belongs to this output process.
        /// </summary>
        string BoxNumber { get; }

        /// <summary>
        /// Gets the list of defined output criteria.
        /// </summary>
        ICriteria[] Criteria { get; }

        #endregion

        #region Events

        /// <summary>
        /// Event which is raised when the output process finished execution.
        /// </summary>
        /// <param name="sender">The output process instance which raised the event.</param>
        /// <param name="e">Not used. Always null.</param>
        event EventHandler Finished;

        /// <summary>
        /// Event which is raised when the output process has box released.
        /// </summary>
        /// <param name="sender">The output process instance which raised the event.</param>
        /// <param name="e">Not used. Always null.</param>
        event EventHandler BoxReleased;

        #endregion

        /// <summary>
        /// Adds a new output criteria element to the output process.
        /// An output process can have multiple output criteria with any filter combination.
        /// </summary>
        /// <param name="articleId">
        /// The unique article identifier (e.g. PZN or EAN) filter criterion of the requested packs.
        /// </param>
        /// <param name="quantity">
        /// The amount of full packs to dispense.
        /// </param>
        /// <param name="batchNumber">
        /// The optional additional batch number filter criterion for the requested packs.
        /// </param>
        /// <param name="externalId">
        /// The optional additional external identifier filter criterion for the requested packs.
        /// </param>
        /// <param name="minimumExpiryDate">
        /// The optional additional filter criterion to request only packs that have at least the specified expiry date.
        /// </param>
        /// <param name="packId">
        /// The optional additional filter criterion which refers to the storage system internal pack identifier.
        /// Because of the fact that every pack in the storage system has a unique pack identifier,
        /// this filter criterion provides the possibility to dispense a specific pack.
        /// </param>
        /// <param name="subItemQuantity">
        /// The number of elements (e.g. pills or ampoules) to dispense. 
        /// If this parameter is set to a value greater than 0, the Quantity property is ignored and should be 0.
        /// </param>
        /// <param name="stockLocationId">
        /// The optional additional stock location filter criterion for the requested packs.
        /// </param>
        /// <param name="machineLocation">
        /// The optional additional machine location filter criterion for the requested packs.
        /// </param>
        /// <param name="singleBatchNumber">
        /// The optional additional flag that all of the requested packs have to belong to the same batch number.
        /// </param>
        void AddCriteria(string articleId,
                         uint quantity,
                         string batchNumber = null,
                         string externalId = null,
                         Nullable<DateTime> minimumExpiryDate = null,
                         ulong packId = 0,
                         uint subItemQuantity = 0,
                         string stockLocationId = null,
                         string machineLocation = null,
                         bool singleBatchNumber = false);

        /// <summary>
        /// Starts the output process by sending the according request to the storage system.
        /// </summary>
        void Start();

        /// <summary>
        /// Requests the cancellation of the output process.
        /// The cancellation is an asynchronous process and therefore a running output process 
        /// may need some time to finish with the state "Aborted". The time required for the 
        /// cancellation heavily depends on the dispensing behavior of the storage system.
        /// </summary>
        void Cancel();
    }
}
