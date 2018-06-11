namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Defines the different possible results of a finished input request.
    /// </summary>
    public enum InputResult
    {
        /// <summary>
        /// The pack input finished successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// The pack input has been aborted.
        /// </summary>
        Aborted
    }
}
