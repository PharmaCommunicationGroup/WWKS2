
namespace CareFusion.Lib.StorageSystem.Stock
{
    /// <summary>
    /// Interface which defines the methods and properties of a storage system article.
    /// </summary>
    public interface IArticle
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier of the article (e.g. PZN or EAN).
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the name of the article.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the dosage form of the article.
        /// </summary>
        string DosageForm { get; }

        /// <summary>
        /// Gets the packaging unit of the article.
        /// </summary>
        string PackagingUnit { get; }

        /// <summary>
        /// Gets the optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </summary>
        uint MaxSubItemQuantity { get; }

        /// <summary>
        /// Gets the number of packs which belong to this article and are currently stored in the storage system.
        /// </summary>
        uint PackCount { get; }

        /// <summary>
        /// Gets the detailed information of the packs which belong to this article and are currently stored in the storage system.
        /// </summary>
        IPack[] Packs { get; }

        #endregion
    }
}
