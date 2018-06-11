using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.Stock;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Pack datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Pack : IPack, IInputPack, IDispensedPack
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Index { get; set; }

        [XmlAttribute]
        public string ScanCode { get; set; }

        [XmlAttribute]
        public string DeliveryNumber { get; set; }

        [XmlAttribute]
        public string BatchNumber { get; set; }

        [XmlAttribute]
        public string ExternalId { get; set; }

        [XmlAttribute]
        public string ExpiryDate { get; set; }

        [XmlAttribute]
        public string StockInDate { get; set; }

        [XmlAttribute]
        public string SubItemQuantity { get; set; }

        [XmlAttribute]
        public string Depth { get; set; }

        [XmlAttribute]
        public string Width { get; set; }

        [XmlAttribute]
        public string Height { get; set; }

        [XmlAttribute]
        public string Shape { get; set; }

        [XmlAttribute]
        public string State { get; set; }

        [XmlAttribute]
        public string IsInFridge { get; set; }

        [XmlAttribute]
        public string BoxNumber { get; set; }

        [XmlAttribute]
        public string OutputDestination { get; set; }

        [XmlAttribute]
        public string OutputSubDestination { get; set; }

        [XmlAttribute]
        public string OutputPoint { get; set; }

        [XmlAttribute]
        public string LabelStatus { get; set; }

        [XmlAttribute]
        public string StockLocationId { get; set; }

        [XmlAttribute]
        public string MachineLocation { get; set; }

        [XmlElement]
        public Handling Handling { get; set; }

        [XmlElement]
        public Error Error { get; set; }

        #endregion

        #region IPack Specific Properties

        /// <summary>
        /// Gets the unique identifier of the pack.
        /// </summary>
        [XmlIgnore]
        ulong IPack.Id { get { return TypeConverter.ConvertULong(this.Id); } }

        /// <summary>
        /// Gets the delivery number that was used during the input of the pack.
        /// </summary>
        [XmlIgnore]
        string IPack.DeliveryNumber { get { return TextConverter.UnescapeInvalidXmlChars(this.DeliveryNumber);  } }

        /// <summary>
        /// Gets the batchnumber of the pack.
        /// </summary>
        [XmlIgnore]
        string IPack.BatchNumber { get { return TextConverter.UnescapeInvalidXmlChars(this.BatchNumber); } }

        /// <summary>
        /// Gets the external identifier of the pack.
        /// </summary>
        [XmlIgnore]
        string IPack.ExternalId { get { return TextConverter.UnescapeInvalidXmlChars(this.ExternalId); } }
        
        /// <summary>
        /// Gets the expiry date of the pack.
        /// </summary>
        [XmlIgnore]
        DateTime IPack.ExpiryDate
        {
            get { return TypeConverter.ConvertDate(this.ExpiryDate); }
        }

        /// <summary>
        /// Gets the stock in date of the pack.
        /// </summary>
        [XmlIgnore]
        DateTime IPack.StockInDate
        {
            get { return TypeConverter.ConvertDate(this.StockInDate); }
        }

        /// <summary>
        /// Gets the full scan code of the pack.
        /// </summary>
        [XmlIgnore]
        string IPack.ScanCode
        {
            get { return TextConverter.UnescapeInvalidXmlChars(this.ScanCode); }
        }

        /// <summary>
        /// Gets the optional number of elements (e.g. pills or ampoules) which are currently in this pack.
        /// This value is used to identify opened packs which does not contain the full amount of pills or ampoules anymore.
        /// A value of 0 means that the pack is completely filled.
        /// </summary>
        [XmlIgnore]
        uint IPack.SubItemQuantity
        {
            get { return TypeConverter.ConvertUInt(this.SubItemQuantity); }
        }

        /// <summary>
        /// Gets the depth of the pack in millimeter.
        /// </summary>
        [XmlIgnore]
        int IPack.Depth
        {
            get { return TypeConverter.ConvertInt(this.Depth); }
        }

        /// <summary>
        /// Gets the width of the pack in millimeter.
        /// </summary>
        [XmlIgnore]
        int IPack.Width
        {
            get { return TypeConverter.ConvertInt(this.Width); }
        }

        /// <summary>
        /// Gets the height of the pack in millimeter.
        /// </summary>
        [XmlIgnore]
        int IPack.Height
        {
            get { return TypeConverter.ConvertInt(this.Height); }
        }

        /// <summary>
        /// Gets the shape of the pack.
        /// </summary>
        [XmlIgnore]
        PackShape IPack.Shape
        {
            get { return TypeConverter.ConvertEnum<PackShape>(this.Shape, PackShape.Cuboid); }
        }

        /// <summary>
        /// Gets the current state of the pack. It might happen that even if a pack
        /// is stored in a storage system, it is not available for dispensing.
        /// For example if there is currently a maintenance processed at the storage system.
        /// </summary>
        [XmlIgnore]
        PackState IPack.State
        {
            get { return TypeConverter.ConvertEnum<PackState>(this.State, PackState.Available); }
        }

        /// <summary>
        /// Gets a flag whether the pack is stored in a refrigerator.
        /// </summary>
        [XmlIgnore]
        bool IPack.IsInFridge
        {
            get { return TypeConverter.ConvertBool(this.IsInFridge); }
        }

        /// <summary>
        /// Gets the stock location of the pack.
        /// </summary>
        [XmlIgnore]
        string IPack.StockLocationId
        {
            get { return this.StockLocationId != null ? TextConverter.EscapeInvalidXmlChars(this.StockLocationId) : string.Empty; }
        }

        /// <summary>
        /// Gets the machine location of the pack.
        /// Is only relevant for multi machine environments.
        /// </summary>
        [XmlIgnore]
        string IPack.MachineLocation
        {
            get { return this.MachineLocation != null ? TextConverter.EscapeInvalidXmlChars(this.MachineLocation) : string.Empty; }
        }

        #endregion

        #region IInputPack Specific Properties

        /// <summary>
        /// Gets the raw scan code of the pack.
        /// </summary>
        [XmlIgnore]
        string IInputPack.ScanCode { get { return TextConverter.UnescapeInvalidXmlChars(this.ScanCode); } }

        /// <summary>
        /// Gets the batch number of the pack if one was specified by the storage system.
        /// </summary>
        [XmlIgnore]
        string IInputPack.BatchNumber { get { return TextConverter.UnescapeInvalidXmlChars(this.BatchNumber); } }

        /// <summary>
        /// Gets the external identifier of the pack.
        /// </summary>
        [XmlIgnore]
        string IInputPack.ExternalId { get { return TextConverter.UnescapeInvalidXmlChars(this.ExternalId); } }
        
        /// <summary>
        /// Gets the expiry date of the pack if one was specified by the storage system.
        /// If no expiry date was specified, this property is null.
        /// </summary>
        [XmlIgnore]
        DateTime? IInputPack.ExpiryDate
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
        /// Gets the optional number of elements (e.g. pills or ampoules) which are currently in this pack.
        /// This value is used to identify opened packs which does not contain the full amount of pills or ampoules anymore.
        /// A value of 0 means that the pack is completely filled.
        /// </summary>
        [XmlIgnore]
        uint IInputPack.SubItemQuantity
        {
            get { return TypeConverter.ConvertUInt(this.SubItemQuantity); }
        }

        /// <summary>
        /// Gets the stock location to use for the pack which was specified by the storage system.
        /// </summary>
        [XmlIgnore]
        string IInputPack.StockLocationId
        {
            get { return this.StockLocationId != null ? TextConverter.EscapeInvalidXmlChars(this.StockLocationId) : string.Empty; }
        }

        /// <summary>
        /// Gets the machine location to use for the pack.
        /// Is only relevant for multi machine environments.
        /// </summary>
        [XmlIgnore]
        string IInputPack.MachineLocation
        {
            get { return this.MachineLocation != null ? TextConverter.EscapeInvalidXmlChars(this.MachineLocation) : string.Empty; }
        }

        #endregion

        #region IDispensedPack Specific Properties

        /// <summary>
        /// Gets or sets the list of packs which were dispensed by the output process.
        /// This property is set after the output process finished.
        /// </summary>
        [XmlIgnore]
        public string ArticleId { get; set; }

        /// <summary>
        /// If the pack was dispensed to a box within a box system scenario,
        /// this property identifies the box the pack was dispensed to.
        /// </summary>
        [XmlIgnore]
        string IDispensedPack.BoxNumber { get { return TextConverter.UnescapeInvalidXmlChars(this.BoxNumber);  } }

        /// <summary>
        /// Gets the destination that was used to dispense this pack. 
        /// In some error scenarios it might happen that the pack is dispensed to another 
        /// output destination than the requested one (e.g. the output destination was not 
        /// operational when the output process was running).
        /// </summary>
        [XmlIgnore]
        int IDispensedPack.OutputDestination { get { return TypeConverter.ConvertInt(this.OutputDestination); } }

        /// <summary>
        /// Gets the number which may define a more detailed  part of the destination
        /// that was used to dispense the pack (e.g. a specific conveyor belt or slide).
        /// </summary>
        [XmlIgnore]
        int IDispensedPack.OutputPoint { get { return TypeConverter.ConvertInt(this.OutputPoint); } }

        /// <summary>
        /// Gets the labelling status of the dispensed pack. 
        /// Only relevant when pack labelling was requested.
        /// </summary>
        [XmlIgnore]
        LabelState IDispensedPack.LabelState { get { return TypeConverter.ConvertEnum<LabelState>(this.LabelStatus, LabelState.NotLabelled); } }

        #endregion

        #region Additional Properties

        /// <summary>
        /// Gets the article object which is currently assigned to this pack.
        /// This property is used to temporarily store the article related properties for this pack. 
        /// </summary>
        [XmlIgnore]
        public Article Article { get; private set; }

        /// <summary>
        /// Gets the virtual article object which is currently assigned to this pack.
        /// This property is used to temporarily store the parent article related properties for this pack. 
        /// </summary>
        [XmlIgnore]
        public Article ParentArticle { get; private set; }
        
        /// <summary>
        /// Gets the other articles object which are related to the same parent.
        /// This property is used to temporarily store the parent article related properties for this pack. 
        /// </summary>
        [XmlIgnore]
        public List<Article> OtherArticles { get; private set; }
        #endregion

        #region IInputPack Specific Methods

        /// <summary>
        /// Sets the article related information for this pack.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="articleId">Unique identifier of the article.</param>
        /// <param name="articleName">Name of the article.</param>
        /// <param name="dosageForm">Dosage form of the article.</param>
        /// <param name="packagingUnit">Packaging unit of the article.</param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        void IInputPack.SetArticleInformation(string articleId, 
                                              string articleName, 
                                              string dosageForm, 
                                              string packagingUnit, 
                                              uint maxSubItemQuantity)
        {
            if (string.IsNullOrEmpty(articleId))
            {
                throw new ArgumentException("Invalid articleId specified.");
            }

            this.Article = new Article()
            {
                Id = TextConverter.EscapeInvalidXmlChars(articleId),
                Name = string.IsNullOrEmpty(articleName) ? string.Empty : TextConverter.EscapeInvalidXmlChars(articleName),
                DosageForm = string.IsNullOrEmpty(dosageForm) ? string.Empty : TextConverter.EscapeInvalidXmlChars(dosageForm),
                PackagingUnit = string.IsNullOrEmpty(packagingUnit) ? string.Empty : TextConverter.EscapeInvalidXmlChars(packagingUnit),
                MaxSubItemQuantity = maxSubItemQuantity.ToString()
            };
        }

        /// <summary>
        /// Sets the article related information for this pack.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="articleId">Unique identifier of the Virtual article.</param>
        /// <param name="articleName">Name of the article.</param>
        /// <param name="dosageForm">Dosage form of the article.</param>
        /// <param name="packagingUnit">Packaging unit of the article.</param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        void IInputPack.SetVirtualArticleInformation(string articleId,
                                   string articleName,
                                   string dosageForm,
                                   string packagingUnit,
                                   uint maxSubItemQuantity)
        {
            if (string.IsNullOrEmpty(articleId))
            {
                throw new ArgumentException("Invalid articleId specified.");
            }

            this.ParentArticle = new Article()
            {
                Id = TextConverter.EscapeInvalidXmlChars(articleId),
                Name = string.IsNullOrEmpty(articleName) ? string.Empty : TextConverter.EscapeInvalidXmlChars(articleName),
                DosageForm = string.IsNullOrEmpty(dosageForm) ? string.Empty : TextConverter.EscapeInvalidXmlChars(dosageForm),
                PackagingUnit = string.IsNullOrEmpty(packagingUnit) ? string.Empty : TextConverter.EscapeInvalidXmlChars(packagingUnit),
                MaxSubItemQuantity = maxSubItemQuantity.ToString()
            };
        }

        /// <summary>
        /// Sets the article related information for this pack.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="articleId">Unique identifier of the Virtual article.</param>
        /// <param name="articleName">Name of the article.</param>
        /// <param name="dosageForm">Dosage form of the article.</param>
        /// <param name="packagingUnit">Packaging unit of the article.</param>
        /// <param name="maxSubItemQuantity">
        /// Optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        void IInputPack.AddOtherRobotArticleInformation(string articleId,
                                   string articleName,
                                   string dosageForm,
                                   string packagingUnit,
                                   uint maxSubItemQuantity)
        {
            if (string.IsNullOrEmpty(articleId))
            {
                throw new ArgumentException("Invalid articleId specified.");
            }

            if (this.OtherArticles == null)
            {
                this.OtherArticles = new List<Article>();
            }

            this.OtherArticles.Add(new Article()
            {
                Id = TextConverter.EscapeInvalidXmlChars(articleId),
                Name = string.IsNullOrEmpty(articleName) ? string.Empty : TextConverter.EscapeInvalidXmlChars(articleName),
                DosageForm = string.IsNullOrEmpty(dosageForm) ? string.Empty : TextConverter.EscapeInvalidXmlChars(dosageForm),
                PackagingUnit = string.IsNullOrEmpty(packagingUnit) ? string.Empty : TextConverter.EscapeInvalidXmlChars(packagingUnit),
                MaxSubItemQuantity = maxSubItemQuantity.ToString()
            });
        }

        /// <summary>
        /// Sets the pack specific information for this pack.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="batchNumber">Optional batch number of this pack or empty string.</param>
        /// <param name="externalId">Optional external identifier of this pack or empty string.</param>
        void IInputPack.SetPackInformation(string batchNumber, string externalId)
        {
            this.BatchNumber = TextConverter.EscapeInvalidXmlChars(batchNumber);
            this.ExternalId = TextConverter.EscapeInvalidXmlChars(externalId);
        }

        /// <summary>
        /// Sets the pack specific information for this pack.
        /// These information are required by the storage system to ensure the correct behaviour of the
        /// output processes later on. Additionally these information are shown in the UI of the storage system.
        /// </summary>
        /// <param name="batchNumber"
        /// >Optional batch number of this pack or empty string.
        /// </param>
        /// <param name="externalId">
        /// Optional external identifier of this pack or empty string.
        /// </param>
        /// <param name="expiryDate">
        /// Expiry date of this pack. If this parameter is null, the storage system will automatically choose an expiry date.
        /// This paramter will overwrite the originally suggested one of the storage system.
        /// </param>
        /// <param name="subItemQuantity">
        /// Optional number of elements (e.g. pills or ampoules) which are currently in this pack.
        /// This value is used to identify opened packs which does not contain the full amount of pills or ampoules anymore.
        /// A value of 0 means that the pack is completely filled.
        /// This paramter will overwrite the originally suggested one of the storage system.
        /// </param>
        /// <param name="stockLocationId">
        /// Optional stock location to use for this pack.
        /// If this parameter is not null, it will overwrite the originally suggested one of the storage system.
        /// </param>
        void IInputPack.SetPackInformation(string batchNumber, 
                                           string externalId, 
                                           DateTime? expiryDate, 
                                           uint subItemQuantity,
                                           string stockLocationId)
        {
            this.BatchNumber = TextConverter.EscapeInvalidXmlChars(batchNumber);
            this.ExternalId = TextConverter.EscapeInvalidXmlChars(externalId);

            if (expiryDate.HasValue)
            {
                this.ExpiryDate = TypeConverter.ConvertDate(expiryDate.Value);
            }

            if (subItemQuantity > 0)
            {
                this.SubItemQuantity = subItemQuantity.ToString();
            }

            if (stockLocationId != null)
            {
                this.StockLocationId = TextConverter.EscapeInvalidXmlChars(stockLocationId);
            }
        }

        /// <summary>
        /// Defines the input handling for this pack.
        /// The input handling instructs the storage system how to handle the scanned pack (e.g. input the pack).
        /// </summary>
        /// <param name="handling">The handling which describes how to handle the pack.</param>
        void IInputPack.SetHandling(InputHandling handling)
        {
            this.Handling = new Handling() { Input = handling.ToString() };
        }

        /// <summary>
        /// Defines the input handling for this pack.
        /// The input handling for a pack instructs the storage system how to handle the scanned pack (e.g. input the pack).
        /// </summary>
        /// <param name="handling">
        /// The handling which describes how to handle the pack.
        /// </param>
        /// <param name="message">
        /// Additional message to show in the UI of the storage system.
        /// This message is typically used in case of a pack rejection to show a more detailed rejection reason.
        /// </param>
        void IInputPack.SetHandling(InputHandling handling, string message)
        {
            this.Handling = new Handling() { Input = handling.ToString(), Text = TextConverter.EscapeInvalidXmlChars(message) };
        }

        #endregion
    }
}
