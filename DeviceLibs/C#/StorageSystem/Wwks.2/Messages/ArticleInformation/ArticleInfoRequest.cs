using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;
using System;
using System.Linq;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.ArticleInformation
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleInfoRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ArticleInfoRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ArticleInfoRequest ArticleInfoRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleInfoRequest message.
    /// </summary>
    public class ArticleInfoRequest : MessageBase, IArticleInfoRequest
    {
        #region Members

        /// <summary>
        /// Flag whether this article info request is finished.
        /// </summary>
        private bool _isFinished;

        #endregion

        #region WWKS 2.0 Properties

        [XmlElement]
        public Article[] Article { get; set; }

        [XmlAttribute]
        public bool IncludeCrossSellingArticles { get; set; }

        [XmlAttribute]
        public bool IncludeAlternativeArticles { get; set; }

        [XmlAttribute]
        public bool IncludeAlternativePackSizeArticles { get; set; }

        #endregion

        #region Additional Properties

        /// <summary>
        /// Gets or sets the message object stream which is used to send the article info response.
        /// </summary>
        [XmlIgnore]
        public XmlObjectStream MessageObjectStream { get; set; }

        #endregion

        #region IArticleInfoRequest Specific Properties

        /// <summary>
        /// Flag determining whether a list of cross-selling articles is being requested.
        /// </summary>
        bool IArticleInfoRequest.IncludeCrossSellingArticles
        {
            get
            {
                return this.IncludeCrossSellingArticles;
            }
        }

        /// <summary>
        /// Flag determining whether a list of alternative is being requested.
        /// </summary>
        bool IArticleInfoRequest.IncludeAlternativeArticles
        {
            get
            {
                return this.IncludeAlternativeArticles;
            }
        }

        /// <summary>
        /// Flag determining whether a list of articles with alternative pack size is being requested.
        /// </summary>
        bool IArticleInfoRequest.IncludeAlternativePackSizeArticles
        {
            get
            {
                return this.IncludeAlternativePackSizeArticles;
            }
        }

        /// <summary>
        /// Gets the list of articles for which general information is being requested.
        /// </summary>
        IDigitalShelfArticle[] IArticleInfoRequest.Articles
        {
            get
            {
                return (this.Article != null) ? (IDigitalShelfArticle[])this.Article.ToArray() : new IDigitalShelfArticle[0];
            }
        }

        #endregion

        #region IArticleInfoRequest Specific Methods

        /// <summary>
        /// Finishes the article price request by sending the corresponding response to the digital shelf system.
        /// This method has to be called after the article information has been added to the articles.
        /// </summary>
        void IArticleInfoRequest.Finish()
        {
            if (!_isFinished)
            {
                var response = new ArticleInfoResponseEnvelope()
                {
                    ArticleInfoResponse = new ArticleInfoResponse()
                    {
                        Id = this.Id,
                        Source = this.Destination,
                        Destination = this.Source,
                        Article = this.Article
                    }
                };

                if (!this.MessageObjectStream.Write(response))
                {
                    throw new ApplicationException("Sending 'ArticleInfoResponse' to digital shelf failed.");
                }

                _isFinished = true;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleInfoRequest"/> class.
        /// </summary>
        public ArticleInfoRequest()
        {
            this.IncludeCrossSellingArticles = false;
            this.IncludeAlternativeArticles = false;
            this.IncludeAlternativePackSizeArticles = false;
        }

    }
}
