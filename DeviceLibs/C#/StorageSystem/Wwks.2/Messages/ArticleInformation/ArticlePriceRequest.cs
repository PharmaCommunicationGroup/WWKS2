using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;
using System;
using System.Linq;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.ArticleInformation
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ArticlePriceRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ArticlePriceRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ArticlePriceRequest ArticlePriceRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ArticlePriceRequest message.
    /// </summary>
    public class ArticlePriceRequest : MessageBase, IArticlePriceRequest
    {
        #region Members

        /// <summary>
        /// Flag whether this article price request is finished.
        /// </summary>
        private bool _isFinished;

        #endregion

        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Currency { get; set; }

        [XmlElement]
        public Article[] Article { get; set; }

        #endregion

        #region Additional Properties

        /// <summary>
        /// Gets or sets the message object stream which is used to send the article price response.
        /// </summary>
        [XmlIgnore]
        public XmlObjectStream MessageObjectStream { get; set; }

        #endregion

        #region IArticlePriceRequest Specific Properties

        /// <summary>
        /// Gets the list of articles for which price information is being requested.
        /// </summary>
        IDigitalShelfArticle[] IArticlePriceRequest.Articles
        {
            get
            {
                return (this.Article != null) ? (IDigitalShelfArticle[])this.Article.ToArray() : new IDigitalShelfArticle[0];
            }
        }

        #endregion

        #region IArticlePriceRequest Specific Methods

        /// <summary>
        /// Finishes the article price request by sending the corresponding response to the digital shelf system.
        /// This method has to be called after the price information has been added to the articles.
        /// </summary>
        void IArticlePriceRequest.Finish()
        {
            if (!_isFinished)
            {
                var response = new ArticlePriceResponseEnvelope()
                {
                    ArticlePriceResponse = new ArticlePriceResponse()
                    {
                        Id = this.Id,
                        Source = this.Destination,
                        Destination = this.Source,
                        Currency = this.Currency,
                        Article = this.Article
                    }
                };

                if (!this.MessageObjectStream.Write(response))
                {
                    throw new ApplicationException("Sending 'ArticlePriceResponse' to digital shelf failed.");
                }

                _isFinished = true;
            }
        }

        #endregion
    }
}
