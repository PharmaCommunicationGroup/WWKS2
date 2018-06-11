using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Stock;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Stock;

namespace CareFusion.Lib.StorageSystem
{
    /// <summary>
    /// Callback definition for an event which is raised whenever a digital shelf system requests article related information from the It-System (e.g. article price).
    /// </summary>
    /// <param name="sender">The <see cref="IDigitalShelf"/> instance which raised the event.</param>
    /// <param name="state">An <see cref="IDigitalShelfArticle"/> instance providing information about the article that has been selected.</param>
    public delegate void ArticleSelectedEventHandler(IDigitalShelf sender, IDigitalShelfArticle article);

    /// <summary>
    /// Callback definition for an event which is raised whenever a digital shelf system requests general article information.
    /// </summary>
    /// <param name="sender">The <see cref="IDigitalShelf"/> instance which raised the event.</param>
    /// <param name="request">An <see cref="IArticleInfoRequest"/> instance providing information about the article for which general information is being requested.</param>
    public delegate void ArticleInfoRequestEventHandler(IDigitalShelf sender, IArticleInfoRequest request);

    /// <summary>
    /// Callback definition for an event which is raised whenever a digital shelf system requests article price information.
    /// </summary>
    /// <param name="sender">The <see cref="IDigitalShelf"/> instance which raised the event.</param>
    /// <param name="request">An <see cref="IArticlePriceRequest"/> instance providing information about the article for which price information is being requested.</param>
    public delegate void ArticlePriceRequestEventHandler(IDigitalShelf sender, IArticlePriceRequest request);

    /// <summary>
    /// Callback definition for an event which is raised whenever a digital shelf system requests a shopping cart (new or existing).
    /// </summary>
    /// <param name="sender">The <see cref="IDigitalShelf"/> instance which raised the event.</param>
    /// <param name="request">An <see cref="IShoppingCartRequest"/> instance providing criteria for the shopping cart which is being requested.</param>
    public delegate void ShoppingCartRequestEventHandler(IDigitalShelf sender, IShoppingCartRequest request);

    /// <summary>
    /// Callback definition for an event which is raised whenever a digital shelf system wants to update an existing shopping cart.
    /// </summary>
    /// <param name="sender">The <see cref="IDigitalShelf"/> instance which raised the event.</param>
    /// <param name="request">An <see cref="IShoppingCartRequest"/> instance providing detailed information about the shopping cart which is to be updated.</param>
    public delegate void ShoppingCartUpdateRequestEventHandler(IDigitalShelf sender, IShoppingCartUpdateRequest request);

    /// <summary>
    /// Callback definition for an event which is raised whenever a digital shelf system wants to request the global stock of the PIS.
    /// </summary>
    /// <param name="sender">The <see cref="IDigitalShelf"/> instance which raised the event.</param>
    /// <param name="request">An <see cref="IStockInfoRequest"/> instance providing detailed information about the Stock Request.</param>
    public delegate void StockInfoRequestEventHandler(IDigitalShelf sender, IStockInfoRequest request);
}
