
namespace CareFusion.Lib.StorageSystem.State
{
    /// <summary>
    /// Defines the different states of a storage system.
    /// </summary>
    public enum ComponentState
    {
        /// <summary>
        /// The storage system is currently not connected.
        /// </summary>
        NotConnected,

        /// <summary>
        /// The connected storage system is ready.
        /// </summary>
        Ready,

        /// <summary>
        /// The connected storage system is not ready.
        /// </summary>
        NotReady
    }
}
