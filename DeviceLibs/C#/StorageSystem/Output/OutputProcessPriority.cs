
namespace CareFusion.Lib.StorageSystem.Output
{
    /// <summary>
    /// Defines the different supported output process priorities.
    /// </summary>
    public enum OutputProcessPriority
    {
        /// <summary>
        /// The output is processed with the lowest priority.
        /// </summary>
        Low,

        /// <summary>
        /// The output is processed with the default priority.
        /// </summary>
        Normal,

        /// <summary>
        /// The output is processed with the highest possible priority.
        /// </summary>
        High
    }
}
