
namespace CareFusion.Lib.StorageSystem.Input
{
    /// <summary>
    /// Interface which defines the methods and properties of a storage system master article.
    /// </summary>
    public interface IMasterArticle 
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
        /// Gets a flag whether packs of this article have to be stored in a refrigerator.
        /// </summary>
        bool RequiresFridge { get; }

        /// <summary>
        /// Gets the optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </summary>
        uint MaxSubItemQuantity { get; }

        /// <summary>
        /// Gets the stock location which is allowed to use for packs of this article.
        /// </summary>
        string StockLocationId { get; }

        /// <summary>
        /// Gets the machine location which is allowed to use for packs of this article.
        /// </summary>
        string MachineLocation { get; }

        /// <summary>
        /// Adds a Child Article to this article.
        /// </summary>
        void AddChildArticle(IMasterArticle childArticle);

        #endregion
    }
}
