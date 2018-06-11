
namespace CareFusion.Lib.StorageSystem.Output
{
    /// <summary>
    /// Defines the different states of an output process.
    /// </summary>
    public enum OutputProcessState
    {
        /// <summary>
        /// The output process was just created and not started yet.
        /// </summary>
        Created,

        /// <summary>
        /// The output process is queued by the storage system and will be started as soon as possible.
        /// </summary>
        Queued,

        /// <summary>
        /// The output process is currently running.
        /// </summary>
        InProcess,

        /// <summary>
        /// The output process has been rejected by the storage system.
        /// </summary>
        Rejected,

        /// <summary>
        /// The output process has been finished successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// The output process has been finished, but not all requested packs were dispensed.
        /// </summary>
        Incomplete,

        /// <summary>
        /// The output process has been aborted.
        /// </summary>
        Aborted,

        /// <summary>
        /// The output process is currently aborting and not finished yet.
        /// </summary>
        Aborting,

        /// <summary>
        /// The output process has a box released
        /// </summary>
        BoxReleased,

        /// <summary>
        /// The output process state is currently unknown.
        /// </summary>
        Unknown
        
    }
}
