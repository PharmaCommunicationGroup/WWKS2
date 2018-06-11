
namespace CareFusion.Lib.StorageSystem.Output
{
    /// <summary>
    /// Defines the different labelling states of a dispensed pack.
    /// </summary>
    public enum LabelState
    {
        /// <summary>
        /// The pack was not labelled (e.g. no label printer is involved).
        /// </summary>
        NotLabelled,

        /// <summary>
        /// The pack was successfully labelled.
        /// </summary>
        Labelled,

        /// <summary>
        /// An error occurred during the labelling of the pack.
        /// </summary>
        LabelError
    }
}
