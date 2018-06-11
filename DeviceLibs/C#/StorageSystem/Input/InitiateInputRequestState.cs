
namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Defines the different states of an initiated input request.
    /// </summary>
    public enum InitiateInputRequestState
    {
        /// <summary>
        /// The initiated input state is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The initiated input has just been created.
        /// </summary>
        Created,

        /// <summary>
        /// The initiated input has been accepted and started.
        /// </summary>
        Accepted,

        /// <summary>
        /// The initiated input was rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// The initiated input successfully finished.
        /// </summary>
        Completed,

        /// <summary>
        /// The initiated input finished with errors.
        /// </summary>
        Incomplete

    }
}
