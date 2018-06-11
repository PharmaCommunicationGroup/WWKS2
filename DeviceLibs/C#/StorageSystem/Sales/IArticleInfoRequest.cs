namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of an article info request.
    /// </summary>
    public interface IArticleInfoRequest
    {
        /// <summary>
        /// Flag determining whether a list of cross-selling articles is being requested.
        /// </summary>
        bool IncludeCrossSellingArticles { get; }

        /// <summary>
        /// Flag determining whether a list of alternative is being requested.
        /// </summary>
        bool IncludeAlternativeArticles { get; }

        /// <summary>
        /// Flag determining whether a list of articles with alternative pack size is being requested.
        /// </summary>
        bool IncludeAlternativePackSizeArticles { get; }

        /// <summary>
        /// Gets the list of articles for which general information is being requested.
        /// </summary>
        IDigitalShelfArticle[] Articles { get; }

        /// <summary>
        /// Finishes the article price request by sending the corresponding response to the digital shelf system.
        /// This method has to be called after the article information has been added to the articles.
        /// </summary>
        void Finish();
    }
}
