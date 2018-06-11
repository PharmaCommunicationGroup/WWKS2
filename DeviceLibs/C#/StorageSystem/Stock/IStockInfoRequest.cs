using CareFusion.Lib.StorageSystem.Output;

namespace CareFusion.Lib.StorageSystem.Stock
{
    /// <summary>
    /// Interface which defines the methods and properties of an stock info request.
    /// </summary>
    public interface IStockInfoRequest
    {
        #region Properties
        string IncludePacks { get; }

        string IncludeArticleDetails { get; }

        ICriteria[] Criterias { get; }

        #endregion


        #region Methods

        /// <summary>
        /// Sets the article related information.
        /// </summary>
        /// <param name="articleId">Unique identifier of the article.</param>
        /// <param name="articleName">Name of the article.</param>
        /// <param name="dosageForm">Dosage form of the article.</param>
        /// <param name="packagingUnit">Packaging unit of the article.</param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        IArticleStockRequest CreateArticleStockRequest(string articleId,
            string articleName = "",
            string dosageForm = "",
            string packagingUnit = "",
            uint maxSubItemQuantity = 0);

        /// <summary>
        /// Finishes the stock request by sending the corresponding response to the digital shelf system.
        /// This method has to be called after the articles has been created and packs has been added to the articles.
        /// </summary>
        void Finish();
        #endregion

    }
}
