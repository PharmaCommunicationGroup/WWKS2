
namespace CareFusion.Lib.StorageSystem.Output
{
    /// <summary>
    /// Interface which defines the methods and properties of a label which
    /// can be defined as part of an output process criteria.
    /// </summary>
    public interface ILabel
    {
        #region Properties

        /// <summary>
        /// Gets the identifier of the label template which has to be used by 
        /// the label printer to process the label content correctly.
        /// </summary>
        string TemplateId { get; }

        /// <summary>
        /// Gets the arbitrary label content to use when printing labels on the packs.
        /// </summary>
        string Content { get; }

        #endregion
    }
}
