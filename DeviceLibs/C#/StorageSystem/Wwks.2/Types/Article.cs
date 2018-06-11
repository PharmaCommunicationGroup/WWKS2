using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Stock;
using CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Article datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Article : IMasterArticle, IArticle, IStockDeliveryItem, IDigitalShelfArticle, IArticleStockRequest
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string DosageForm { get; set; }

        [XmlAttribute]
        public string PackagingUnit { get; set; }

        [XmlAttribute]
        public string BatchNumber { get; set; }

        [XmlAttribute]
        public string ExternalId { get; set; }

        [XmlAttribute]
        public string ExpiryDate { get; set; }

        [XmlAttribute]
        public string RequiresFridge { get; set; }

        [XmlAttribute]
        public string Quantity { get; set; }

        [XmlAttribute]
        public string MaxSubItemQuantity { get; set; }

        [XmlAttribute]
        public string StockLocationId { get; set; }

        [XmlAttribute]
        public string MachineLocation { get; set; }

        [XmlAttribute]
        public string PatientInformationLeaflet { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlElement]
        public List<Pack> Pack { get; set; }

        [XmlElement]
        public List<Article> ChildArticle { get; set; }
        
        [XmlElement]
        public List<Tag> Tags { get; set; }

        [XmlElement]
        public List<PriceInformation> PriceInformation { get; set; }
        
        [XmlElement]
        public CrossSellingArticles CrossSellingArticles { get; set; }

        [XmlElement]
        public AlternativeArticles AlternativeArticles { get; set; }

        [XmlElement]
        public AlternativePackSizeArticles AlternativePackSizeArticles { get; set; }

        #endregion

        #region IMasterArticle Specific Properties

        /// <summary>
        /// Gets the unique identifier of the article (e.g. PZN or EAN).
        /// </summary>
        [XmlIgnore]
        string IMasterArticle.Id { get { return TextConverter.UnescapeInvalidXmlChars(this.Id); } }

        /// <summary>
        /// Gets the name of the article.
        /// </summary>
        [XmlIgnore]
        string IMasterArticle.Name { get { return TextConverter.UnescapeInvalidXmlChars(this.Name); } }

        /// <summary>
        /// Gets the dosage form of the article.
        /// </summary>
        [XmlIgnore]
        string IMasterArticle.DosageForm { get { return TextConverter.UnescapeInvalidXmlChars(this.DosageForm); } }

        /// <summary>
        /// Gets the packaging unit of the article.
        /// </summary>
        [XmlIgnore]
        string IMasterArticle.PackagingUnit { get { return TextConverter.UnescapeInvalidXmlChars(this.PackagingUnit); } }

        /// <summary>
        /// Gets a flag whether packs of this article have to be stored in a refrigerator.
        /// </summary>
        [XmlIgnore]
        bool IMasterArticle.RequiresFridge
        {
            get { return TypeConverter.ConvertBool(this.RequiresFridge); }
        }

        /// <summary>
        /// Gets the optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </summary>
        [XmlIgnore]
        uint IMasterArticle.MaxSubItemQuantity
        {
            get { return TypeConverter.ConvertUInt(this.MaxSubItemQuantity); }
        }

        /// <summary>
        /// Gets the stock location which is allowed to use for packs of this article.
        /// </summary>
        [XmlIgnore]
        string IMasterArticle.StockLocationId
        {
            get { return this.StockLocationId != null ? TextConverter.EscapeInvalidXmlChars(this.StockLocationId) : string.Empty; }
        }

        /// <summary>
        /// Gets the machine location which is allowed to use for packs of this article.
        /// </summary>
        [XmlIgnore]
        string IMasterArticle.MachineLocation
        {
            get { return this.MachineLocation != null ? TextConverter.EscapeInvalidXmlChars(this.MachineLocation) : string.Empty; }
        }

        /// <summary>
        /// Adds a Child Article to this article.
        /// </summary>
        void IMasterArticle.AddChildArticle(IMasterArticle childArticle)
        {
            Article newArticle = new Article();
            newArticle.Id = childArticle.Id;
            newArticle.Name = childArticle.Name;
            newArticle.DosageForm = childArticle.DosageForm;
            newArticle.PackagingUnit = childArticle.PackagingUnit;
            newArticle.RequiresFridge = childArticle.RequiresFridge.ToString();
            newArticle.MaxSubItemQuantity = childArticle.MaxSubItemQuantity.ToString();
            newArticle.StockLocationId = childArticle.StockLocationId;
            newArticle.MachineLocation = childArticle.MachineLocation;

            if (ChildArticle == null)
            {
                ChildArticle = new List<Article>();
            }

            ChildArticle.Add(newArticle);
        }

        #endregion

        #region IArticle Specific Properties

        /// <summary>
        /// Gets the unique identifier of the article (e.g. PZN or EAN).
        /// </summary>
        [XmlIgnore]
        string IArticle.Id { get { return TextConverter.UnescapeInvalidXmlChars(this.Id); } }

        /// <summary>
        /// Gets the name of the article.
        /// </summary>
        [XmlIgnore]
        string IArticle.Name { get { return TextConverter.UnescapeInvalidXmlChars(this.Name); } }

        /// <summary>
        /// Gets the dosage form of the article.
        /// </summary>
        [XmlIgnore]
        string IArticle.DosageForm { get { return TextConverter.UnescapeInvalidXmlChars(this.DosageForm); } }

        /// <summary>
        /// Gets the packaging unit of the article.
        /// </summary>
        [XmlIgnore]
        string IArticle.PackagingUnit { get { return TextConverter.UnescapeInvalidXmlChars(this.PackagingUnit); } }

        /// <summary>
        /// Gets the optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </summary>
        [XmlIgnore]
        uint IArticle.MaxSubItemQuantity
        {
            get { return TypeConverter.ConvertUInt(this.MaxSubItemQuantity); }
        }

        /// <summary>
        /// Gets the number of packs which belong to this article and are currently stored in the storage system.
        /// </summary>
        [XmlIgnore]
        uint IArticle.PackCount
        {
            get { return TypeConverter.ConvertUInt(this.Quantity); }
        }

        /// <summary>
        /// Gets the detailed information of the packs which belong to this article and are currently stored in the storage system.
        /// </summary>
        [XmlIgnore]
        IPack[] IArticle.Packs
        {
            get { return (this.Pack != null) ? (IPack[])this.Pack.ToArray() : new IPack[0]; }
        }

        #endregion

        #region IStockDeliveryItem Specific Properties

        /// <summary>
        /// Gets the unique identifier of the article (e.g. PZN or EAN).
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryItem.ArticleId
        {
            get { return TextConverter.UnescapeInvalidXmlChars(this.Id); }
        }

        /// <summary>
        /// Gets the name of the article.
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryItem.ArticleName
        {
            get { return TextConverter.UnescapeInvalidXmlChars(this.Name); }
        }

        /// <summary>
        /// Gets the dosage form of the article.
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryItem.DosageForm { get { return TextConverter.UnescapeInvalidXmlChars(this.DosageForm); } }

        /// <summary>
        /// Gets the packaging unit of the article.
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryItem.PackagingUnit { get { return TextConverter.UnescapeInvalidXmlChars(this.PackagingUnit); } }

        /// <summary>
        /// Gets the batch number which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with the according stock delivery.
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryItem.BatchNumber { get { return TextConverter.UnescapeInvalidXmlChars(this.BatchNumber); } }

        /// <summary>
        /// Gets the external identifier which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with the according stock delivery.
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryItem.ExternalId { get { return TextConverter.UnescapeInvalidXmlChars(this.ExternalId); } }

        /// <summary>
        /// Gets a flag whether packs of this article have to be stored in a refrigerator.
        /// </summary>
        [XmlIgnore]
        bool IStockDeliveryItem.RequiresFridge
        {
            get { return TypeConverter.ConvertBool(this.RequiresFridge); }
        }

        /// <summary>
        /// Gets the expiry date which will be assigned to each pack of this article 
        /// that is input into the storage system in combination with the according stock delivery.
        /// If this property is null, the storage system will automatically choose an expiry date.
        /// </summary>
        [XmlIgnore]
        DateTime? IStockDeliveryItem.ExpiryDate
        {
            get 
            {
                if (string.IsNullOrEmpty(this.ExpiryDate))
                {
                    return null;
                }

                return TypeConverter.ConvertDate(this.ExpiryDate);            
            }
        }

        /// <summary>
        /// Optional limitation of how many packs of this article are allowed 
        /// to be stored in the storage system in combination with the according stock delivery.
        /// </summary>
        [XmlIgnore]
        uint IStockDeliveryItem.MaxAllowedPackCount
        {
            get { return TypeConverter.ConvertUInt(this.Quantity); }
        }

        /// <summary>
        /// Gets the optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </summary>
        [XmlIgnore]
        uint IStockDeliveryItem.MaxSubItemQuantity
        {
            get { return TypeConverter.ConvertUInt(this.MaxSubItemQuantity); }
        }

        /// <summary>
        /// Gets the stock location which is allowed to use for packs of this article.
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryItem.StockLocationId
        {
            get { return this.StockLocationId != null ? TextConverter.EscapeInvalidXmlChars(this.StockLocationId) : string.Empty; }
        }

        /// <summary>
        /// Gets the machine location which is allowed to use for packs of this article.
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryItem.MachineLocation
        {
            get { return this.MachineLocation != null ? TextConverter.EscapeInvalidXmlChars(this.MachineLocation) : string.Empty; }
        }

        #endregion

        #region IDigitalShelfArticle Specific Properties

        /// <summary>
        /// Gets the unique identifier of the article (e.g. PZN or EAN).
        /// </summary>
        [XmlIgnore]
        string IDigitalShelfArticle.Id
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.Id);
            }
        }

        /// <summary>
        /// Gets the name of the article.
        /// </summary>
        [XmlIgnore]
        string IDigitalShelfArticle.Name
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.Name);
            }
        }

        /// <summary>
        /// Gets the dosage form of the article.
        /// </summary>
        [XmlIgnore]
        string IDigitalShelfArticle.DosageForm
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.DosageForm);
            }
        }

        /// <summary>
        /// Gets the packaging unit of the article.
        /// </summary>
        [XmlIgnore]
        string IDigitalShelfArticle.PackagingUnit
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.PackagingUnit);
            }
        }

        /// <summary>
        /// Gets the optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </summary>
        [XmlIgnore]
        uint IDigitalShelfArticle.MaxSubItemQuantity
        {
            get
            {
                return TypeConverter.ConvertUInt(this.MaxSubItemQuantity);
            }
        }

        /// <summary>
        /// Gets the optional patient information leaflet of the article.
        /// </summary>
        [XmlIgnore]
        string IDigitalShelfArticle.PatientInformationLeaflet
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.PatientInformationLeaflet);
            }
        }

        /// <summary>
        /// Gets the optional description of the article.
        /// </summary>
        [XmlIgnore]
        string IDigitalShelfArticle.Description
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.Description);
            }
        }

        /// <summary>
        /// Gets the tags of the article.
        /// </summary>
        [XmlIgnore]
        ITag[] IDigitalShelfArticle.Tags
        {
            get
            {
                return (this.Tags != null) ? (ITag[])this.Tags.ToArray() : new ITag[0];
            }
        }

        /// <summary>
        /// Gets the corresponding cross-selling articles for the article.
        /// </summary>
        [XmlIgnore]
        IArticle[] IDigitalShelfArticle.CrossSellingArticles
        {
            get
            {
                return (this.CrossSellingArticles != null) ? (IArticle[])this.CrossSellingArticles.Article.ToArray() : new IArticle[0];
            }
        }

        /// <summary>
        /// Gets the corresponding alternative articles for the article.
        /// </summary>
        [XmlIgnore]
        IArticle[] IDigitalShelfArticle.AlternativeArticles
        {
            get
            {
                return (this.AlternativeArticles != null) ? (IArticle[])this.AlternativeArticles.Article.ToArray() : new IArticle[0];
            }
        }

        /// <summary>
        /// Gets the corresponding alternative pack size articles for the article.
        /// </summary>
        [XmlIgnore]
        IArticle[] IDigitalShelfArticle.AlternativePackSizeArticles
        {
            get
            {
                return (this.AlternativePackSizeArticles != null) ? (IArticle[])this.AlternativePackSizeArticles.Article.ToArray() : new IArticle[0];
            }
        }

        /// <summary>
        /// Gets the price(s) of the article. This might be more than one price (depending on quantity, specials offers etc.).
        /// </summary>
        IPriceInformation[] IDigitalShelfArticle.PriceInformation
        {
            get
            {
                return (this.PriceInformation != null) ? (IPriceInformation[])this.PriceInformation.ToArray() : new IPriceInformation[0];
            }
        }

        #endregion

        #region IDigitalShelfArticle Specific Methods

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
        void IDigitalShelfArticle.SetArticleInformation(string articleId, 
                                              string articleName,
                                              string dosageForm,
                                              string packagingUnit,
                                              uint maxSubItemQuantity)
        {
            Id = TextConverter.EscapeInvalidXmlChars(articleId);
            Name = string.IsNullOrEmpty(articleName) ? string.Empty : TextConverter.EscapeInvalidXmlChars(articleName);
            DosageForm = string.IsNullOrEmpty(dosageForm) ? string.Empty : TextConverter.EscapeInvalidXmlChars(dosageForm);
            PackagingUnit = string.IsNullOrEmpty(packagingUnit) ? string.Empty : TextConverter.EscapeInvalidXmlChars(packagingUnit);
            MaxSubItemQuantity = maxSubItemQuantity.ToString();
        }


        /// <summary>
        /// Adds an article tag (e.g. "discrete" or "profit").
        /// </summary>
        /// <param name="value">The value of the article tag.</param>
        void IDigitalShelfArticle.AddTag(string value)
        {
            if (this.Tags == null)
            {
                this.Tags = new List<Tag>();
            }

            var tag = new Tag()
            {
                Value = TextConverter.EscapeInvalidXmlChars(value)
            };

            this.Tags.Add(tag);
        }

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
        void IDigitalShelfArticle.AddPriceInformation(PriceCategory category, decimal price, uint quantity, decimal basePrice, string basePriceUnit, string vat, string description)
        {
            if (this.PriceInformation == null)
            {
                this.PriceInformation = new List<PriceInformation>();
            }

            var priceInformation = new PriceInformation()
            {
                Category = TextConverter.EscapeInvalidXmlChars(category.ToString()),
                Price = price.ToString(CultureInfo.InvariantCulture),
                Quantity = quantity.ToString(),
                BasePrice = basePrice.ToString(CultureInfo.InvariantCulture),
                BasePriceUnit = TextConverter.EscapeInvalidXmlChars(basePriceUnit),
                VAT = TextConverter.EscapeInvalidXmlChars(vat),
                Description = TextConverter.EscapeInvalidXmlChars(description)
            };

            this.PriceInformation.Add(priceInformation);
        }

        /// <summary>
        /// Adds a corresponding cross-selling article.
        /// </summary>
        /// <param name="articleId">The unique Id of the cross-selling article to be added.</param>
        void IDigitalShelfArticle.AddCrossSellingArticle(string articleId)
        {
            if (this.CrossSellingArticles == null)
            {
                this.CrossSellingArticles = new CrossSellingArticles();
            }

            var article = new Article()
            {
                Id = TextConverter.EscapeInvalidXmlChars(articleId)
            };

            this.CrossSellingArticles.Article.Add(article);
        }

        /// <summary>
        /// Adds a corresponding alternative article.
        /// </summary>
        /// <param name="articleId">The unique Id of the alternative article to be added.</param>
        void IDigitalShelfArticle.AddAlternativeArticle(string articleId)
        {
            if (this.AlternativeArticles == null)
            {
                this.AlternativeArticles = new AlternativeArticles();
            }

            var article = new Article()
            {
                Id = TextConverter.EscapeInvalidXmlChars(articleId)
            };

            this.AlternativeArticles.Article.Add(article);
        }

        /// <summary>
        /// Adds a corresponding alternative pack size article.
        /// </summary>
        /// <param name="articleId">The unique Id of the alternative pack size article to be added.</param>
        void IDigitalShelfArticle.AddAlternativePackSizeArticle(string articleId)
        {
            if (this.AlternativePackSizeArticles == null)
            {
                this.AlternativePackSizeArticles = new AlternativePackSizeArticles();
            }

            var article = new Article()
            {
                Id = TextConverter.EscapeInvalidXmlChars(articleId)
            };

            this.AlternativePackSizeArticles.Article.Add(article);
        }

        #endregion

        #region IArticleStockRequest Specific Methods

        /// <summary>
        /// Adds a pack to this article stock.
        /// </summary>
        /// <param name="subItemQuantity">
        /// Optional number of elements (e.g. pills or ampoules) which are in one open pack of this article.
        /// A value of 0 means a full pack.
        /// </param>
        void IArticleStockRequest.AddPack(uint subItemQuantity)
        {
            if (Pack == null)
            {
                Pack = new List<Pack>();
            }
            
            Pack.Add(new Pack()
            {
                Id = Pack.Count.ToString(),
                ArticleId = Id,
                SubItemQuantity = subItemQuantity.ToString()
            });
        }

        /// <summary>
        /// Adds a multiple packs to this article stock.
        /// </summary>
        /// <param name="packCount"> Number of packs to add. </param>
        void IArticleStockRequest.AddMultiplePacks(int packCount)
        {
            for (int i = 0; i < packCount; i++)
            {
                (this as IArticleStockRequest).AddPack();
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Article"/> class.
        /// </summary>
        public Article()
        {
            this.Id = string.Empty;
            this.Name = string.Empty;
            this.DosageForm = string.Empty;
            this.PackagingUnit = string.Empty;
        }
    }
}
