namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of an article price request.
    /// </summary>
    public interface IArticlePriceRequest
    {
        /// <summary>
        /// Gets the list of articles for which price information is being requested.
        /// </summary>
        IDigitalShelfArticle[] Articles { get; }

        /// <summary>
        /// Finishes the article price request by sending the corresponding response to the digital shelf system.
        /// This method has to be called after the price information has been added to the articles.
        /// </summary>
        void Finish();
    }
}
