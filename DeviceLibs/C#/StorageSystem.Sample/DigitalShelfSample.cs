using CareFusion.Lib.StorageSystem;
using CareFusion.Lib.StorageSystem.Sales;
using System;
using System.Linq;
using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.Stock;

namespace StorageSystem.Sample
{
    public class DigitalShelfSample
    {
        private IDigitalShelf _digitalShelf;

        public void Run()
        {
            _digitalShelf = new RowaDigitalShelf();
            _digitalShelf.ArticleInfoRequested += DigitalShelf_ArticleInfoRequested;
            _digitalShelf.ArticlePriceRequested += DigitalShelf_ArticlePriceRequested;
            _digitalShelf.ArticleSelected += DigitalShelf_ArticleSelected;
            _digitalShelf.ShoppingCartRequested += DigitalShelf_ShoppingCartRequested;
            _digitalShelf.ShoppingCartUpdateRequested += DigitalShelf_ShoppingCartUpdateRequested;
            _digitalShelf.StockInfoRequested += DigitalShelf_StockInfoRequested;

            Console.WriteLine("Connecting to digital shelf...");

            _digitalShelf.Connect("127.0.0.1");

            Console.WriteLine();
            Console.WriteLine("PRESS ENTER TO QUIT");
            Console.ReadLine();

            _digitalShelf.Disconnect();
            _digitalShelf.Dispose();
            _digitalShelf = null;
        }

        private void DigitalShelf_StockInfoRequested(IDigitalShelf sender, IStockInfoRequest request)
        {
            IArticleStockRequest articleStockRequest = null;

            if (request.Criterias != null)
            {
                foreach (ICriteria criteria in request.Criterias)
                {
                    string articleId = String.IsNullOrEmpty(criteria.ArticleId) ? "123" : criteria.ArticleId;

                    articleStockRequest = request.CreateArticleStockRequest(articleId);
                    articleStockRequest.AddMultiplePacks(5);
                    articleStockRequest.AddPack(1);
                    articleStockRequest.AddPack(2);
                    articleStockRequest.AddPack(3);
                    articleStockRequest.AddPack(4);
                }
            }
            else
            {
                // 123
                articleStockRequest = request.CreateArticleStockRequest("123");
                articleStockRequest.AddPack();
                articleStockRequest.AddPack(1);
                articleStockRequest.AddPack(2);
                articleStockRequest.AddPack(3);
                articleStockRequest.AddPack(4);
                articleStockRequest.AddPack();
                articleStockRequest.AddPack();
                
                // 124
                articleStockRequest = request.CreateArticleStockRequest("124");
                articleStockRequest.AddPack();
                articleStockRequest.AddPack(11);
                articleStockRequest.AddPack(24);
                articleStockRequest.AddPack();
            }

            request.Finish();
            Console.WriteLine(string.Format("Stock Info has been requested from the digital shelf."));
        }

        private void DigitalShelf_ShoppingCartUpdateRequested(IDigitalShelf sender, IShoppingCartUpdateRequest request)
        {
            Console.WriteLine(string.Format("Shopping cart update has been requested from the digital shelf."));

            request.Accept("Shopping cart has been updated successfully");
        }

        private void DigitalShelf_ShoppingCartRequested(IDigitalShelf sender, IShoppingCartRequest request)
        {
            Console.WriteLine(string.Format("Shopping cart has been requested from the digital shelf."));
            var shoppingCart = _digitalShelf.CreateShoppingCart(request.Criteria.ShoppingCartId, ShoppingCartStatus.Active, "CustID", "SaleID", "salePointId", "ViewPointID");

            shoppingCart.AddItem("1", 10, 2, 5, "100", "EUR");
            shoppingCart.AddItem("2", 50, 10, 50, "10", "FR");
            request.Accept(shoppingCart);

            shoppingCart.Status = ShoppingCartStatus.Finished;
            shoppingCart.SalesPointId = "1";
            shoppingCart.ViewPointId = "2";
            shoppingCart.SalesPersonId = "3";
            shoppingCart.CustomerId = "4";
            _digitalShelf.UpdateShoppingCart(shoppingCart);
        }

        private void DigitalShelf_ArticleSelected(IDigitalShelf sender, IDigitalShelfArticle article)
        {
            Console.WriteLine(string.Format("Article '{0} - {1}' has been selected on the digital shelf screen.", article.Id, article.Name));
        }

        private void DigitalShelf_ArticlePriceRequested(IDigitalShelf sender, IArticlePriceRequest request)
        {
            Console.WriteLine(string.Format("Price information for '{0}' articles has been requested from the digital shelf.", request.Articles.Count()));

            foreach (var article in request.Articles)
            {
                article.AddPriceInformation(PriceCategory.RRP, 100);
            }

            request.Finish();
        }

        private void DigitalShelf_ArticleInfoRequested(IDigitalShelf sender, IArticleInfoRequest request)
        {
            Console.WriteLine(string.Format("Article information for '{0}' articles has been requested from the digital shelf.", request.Articles.Count()));

            foreach (var article in request.Articles)
            {
                article.SetArticleInformation(article.Id, "Test Name", "dosage form", "packaging unit", 10);
                article.AddTag("tag");

                if (request.IncludeCrossSellingArticles)
                {
                    article.AddCrossSellingArticle("article-111");
                }
                
                if (request.IncludeAlternativeArticles)
                {
                    article.AddAlternativeArticle("article-222");
                }
                
                if (request.IncludeAlternativePackSizeArticles)
                {
                    article.AddAlternativePackSizeArticle("article-333");
                }
            }

            request.Finish();
        }
    }
}
