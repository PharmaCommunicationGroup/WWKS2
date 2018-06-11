
namespace CareFusion.Lib.StorageSystem.State
{
    /// <summary>
    /// Interface which defines the methods and properties of status component.
    /// </summary>
    public interface IComponent
    {
        #region Properties

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        ComponentType Type { get; }

        /// <summary>
        /// Gets the description of the component.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the state of the component.
        /// </summary>
        ComponentState State { get; }

        /// <summary>
        /// Gets the optional additional state description of the component.
        /// </summary>
        string StateText { get; }

        #endregion
    }
}
