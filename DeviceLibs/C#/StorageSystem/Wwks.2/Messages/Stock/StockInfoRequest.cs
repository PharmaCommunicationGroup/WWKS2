using System;
using System.Collections.Generic;
using CareFusion.Lib.StorageSystem.Stock;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;
using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Input;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Stock
{
    /// <summary>
    /// Class which represents the WWKS 2.0 StockInfoRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class StockInfoRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public StockInfoRequest StockInfoRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 StockInfoRequest message.
    /// </summary>
    public class StockInfoRequest : MessageBase, IStockInfoRequest
    {
        #region Members

        /// <summary>
        /// Flag whether this input request is finished.
        /// </summary>
        private bool _isFinished;

        private List<Article> _articles;
        #endregion

        #region WWKS 2.0 Properties


        #endregion

        #region Additional Properties

        /// <summary>
        /// Gets or sets the message object stream which is used to send the shopping cart update response.
        /// </summary>
        [XmlIgnore]
        public XmlObjectStream MessageObjectStream { get; set; }

        #endregion

        #region Properties

        [XmlAttribute]
        public string IncludePacks { get; set; }

        [XmlAttribute]
        public string IncludeArticleDetails { get; set; }

        [XmlElement]
        public Criteria[] Criteria { get; set; }

        #endregion

        #region IStockInfoRequest Specific Properties

        /// <summary>
        /// Gets the list of packs, the storage system wants to input.
        /// The IT system iterates this pack list, defines further article details
        /// for each pack and decides wether to allow the pack input or not.
        /// Afterwards the method "Finish" is called to complete the input process.
        /// </summary>
        [XmlIgnore]
        ICriteria[] IStockInfoRequest.Criterias { get { return Criteria; } }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="StockInfoRequest"/> class.
        /// </summary>
        public StockInfoRequest()
        {
            this.IncludePacks = "True";
            this.IncludeArticleDetails = "False";
            _articles = new List<Article>();
        }

        #region IStockInfoRequest Specific Methods

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
        IArticleStockRequest IStockInfoRequest.CreateArticleStockRequest(string articleId,
            string articleName,
            string dosageForm,
            string packagingUnit,
            uint maxSubItemQuantity)
        {
            Article article = _articles.Find(b => b.Id == articleId);

            if (article == null)
            {
                article = new Article();
                _articles.Add(article);
            }

            article.Id = TextConverter.EscapeInvalidXmlChars(articleId);
            article.Name = string.IsNullOrEmpty(articleName)
                ? string.Empty
                : TextConverter.EscapeInvalidXmlChars(articleName);
            article.DosageForm = string.IsNullOrEmpty(dosageForm)
                ? string.Empty
                : TextConverter.EscapeInvalidXmlChars(dosageForm);
            article.PackagingUnit = string.IsNullOrEmpty(packagingUnit)
                ? string.Empty
                : TextConverter.EscapeInvalidXmlChars(packagingUnit);
            article.MaxSubItemQuantity = maxSubItemQuantity.ToString();
                
            return article;
        }

        /// <summary>
        /// Finishes the stock request by sending the corresponding response to the digital shelf system.
        /// This method has to be called after the articles has been created and packs has been added to the articles.
        /// </summary>
        void IStockInfoRequest.Finish()
        {
            if (_isFinished)
            {
                return;
            }

            if (MessageObjectStream == null)
            {
                throw new ApplicationException("No Mosaic connection available.");
            }

            var response = new StockInfoResponseEnvelope()
            {
                StockInfoResponse = new StockInfoResponse()
                {
                    Id = this.Id,
                    Source = this.Destination,
                    Destination = this.Source,
                    Article = _articles.ToArray()
                }
            };

            foreach (Article article in response.StockInfoResponse.Article)
            {
                if (article.Pack != null)
                {
                    article.Quantity = article.Pack.Count.ToString();

                    if (this.IncludePacks.ToLower() == "false")
                    {
                        article.Pack.Clear();
                    }
                }
                else
                {
                    article.Quantity = "0";
                }
            }

            if (MessageObjectStream.Write(response) == false)
            {
                throw new ApplicationException("Sending 'InputResponse' to storage system failed.");
            }

            _isFinished = true;
        }

        #endregion
    }
}
