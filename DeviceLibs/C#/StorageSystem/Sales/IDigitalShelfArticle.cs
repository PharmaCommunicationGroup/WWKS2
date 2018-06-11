using CareFusion.Lib.StorageSystem.Stock;

namespace CareFusion.Lib.StorageSystem.Sales
{
    /// <summary>
    /// Interface which defines the methods and properties of a digital shelf article.
    /// </summary>
    public interface IDigitalShelfArticle
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
        /// Gets the optional patient information leaflet of the article.
        /// </summary>
        string PatientInformationLeaflet { get; }

        /// <summary>
        /// Gets the optional description of the article.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the tags of the article.
        /// </summary>
        ITag[] Tags { get; }

        /// <summary>
        /// Gets the price(s) of the article. This might be more than one price (depending on quantity, specials offers etc.).
        /// </summary>
        IPriceInformation[] PriceInformation { get; }

        /// <summary>
        /// Gets the corresponding cross-selling articles for the article.
        /// </summary>
        IArticle[] CrossSellingArticles { get; }

        /// <summary>
        /// Gets the corresponding alternative articles for the article.
        /// </summary>
        IArticle[] AlternativeArticles { get; }

        /// <summary>
        /// Gets the corresponding alternative pack size articles for the article.
        /// </summary>
        IArticle[] AlternativePackSizeArticles { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the article related information for this pack.
        /// </summary>
        /// <param name="articleId">Unique identifier of the article.</param>
        /// <param name="articleName">Name of the article.</param>
        /// <param name="dosageForm">Dosage form of the article.</param>
        /// <param name="packagingUnit">Packaging unit of the article.</param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        void SetArticleInformation(string articleId, 
                                   string articleName,
                                   string dosageForm,
                                   string packagingUnit,
                                   uint maxSubItemQuantity = 0);
        
        /// <summary>
        /// Adds an article tag (e.g. "discrete" or "profit").
        /// </summary>
        /// <param name="value">The value of the article tag.</param>
        void AddTag(string value);

        /// <summary>
        /// Adds price information to the article.
        /// </summary>
        /// <param name="category">The category of the price (e.g. "RRP" or "Offer").</param>
        /// <param name="price">The actual price of the article specified in the smalles unit of the currency (e.g. Cent).</param>
        /// <param name="quantity">The quantity from which the price is valid.</param>
        /// <param name="basePrice">The base price of the article.</param>
        /// <param name="basePriceUnit">The base price is specified in (e.g. "100ml).</param>
        /// <param name="vat">The VAT of the article.</param>
        /// <param name="description">The description of the price information (e.g. name of a special offer).</param>
        void AddPriceInformation(PriceCategory category, decimal price, uint quantity = 1, decimal basePrice = 0, string basePriceUnit = "", string vat = "", string description = "");

        /// <summary>
        /// Adds a corresponding cross-selling article.
        /// </summary>
        /// <param name="articleId">The unique Id of the cross-selling article to be added.</param>
        void AddCrossSellingArticle(string articleId);

        /// <summary>
        /// Adds a corresponding alternative article.
        /// </summary>
        /// <param name="articleId">The unique Id of the alternative article to be added.</param>
        void AddAlternativeArticle(string articleId);

        /// <summary>
        /// Adds a corresponding alternative pack size article.
        /// </summary>
        /// <param name="articleId">The unique Id of the alternative pack size article to be added.</param>
        void AddAlternativePackSizeArticle(string articleId);

        #endregion
    }
}
