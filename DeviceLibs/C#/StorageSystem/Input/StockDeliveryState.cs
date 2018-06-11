
namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Defines the different states of a stock delivery.
    /// </summary>
    public enum StockDeliveryState
    {
        /// <summary>
        /// The stock delivery is queued by the storage system and may be started at any time.
        /// </summary>
        Queued,

        /// <summary>
        /// The stock delivery is currently active.
        /// </summary>
        InProcess,

        /// <summary>
        /// The stock delivery has been finished successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// The stock delivery has been finished, but not all requested packs were stored in the storage system.
        /// </summary>
        Incomplete,

        /// <summary>
        /// The stock delivery has been aborted.
        /// </summary>
        Aborted,

        /// <summary>
        /// The stock delivery is currently aborting and not finished yet.
        /// </summary>
        Aborting,

        /// <summary>
        /// The stock delivery state is currently unknown.
        /// </summary>
        Unknown
    }
}
