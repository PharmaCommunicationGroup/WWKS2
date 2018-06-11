using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Criteria datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Criteria : ICriteria, IShoppingCartCriteria
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string ArticleId { get; set; }

        [XmlAttribute]
        public string Quantity { get; set; }

        [XmlAttribute]
        public string SubItemQuantity { get; set; }

        [XmlAttribute]
        public string MinimumExpiryDate { get; set; }

        [XmlAttribute]
        public string BatchNumber { get; set; }

        [XmlAttribute]
        public string ExternalId { get; set; }

        [XmlAttribute]
        public string PackId { get; set; }

        [XmlAttribute]
        public string StockLocationId { get; set; }

        [XmlAttribute]
        public string MachineLocation { get; set; }

        [XmlAttribute]
        public string SingleBatchNumber { get; set; }

        [XmlElement]
        public List<Label> Label { get; set; }

        [XmlAttribute]
        public string ShoppingCartId { get; set; }

        [XmlAttribute]
        public string SalesPointId { get; set; }

        [XmlAttribute]
        public string ViewPointId { get; set; }

        [XmlAttribute]
        public string SalesPersonId { get; set; }

        [XmlAttribute]
        public string CustomerId { get; set; }

        #endregion       

        #region ICriteria Specific Properties

        /// <summary>
        /// Gets the unique article identifier (e.g. PZN or EAN) filter criterion of the requested packs.
        /// </summary>
        [XmlIgnore]
        string ICriteria.ArticleId { get { return TextConverter.UnescapeInvalidXmlChars(this.ArticleId); } }

        /// <summary>
        /// Gets the optional additional batch number filter criterion for the requested packs. 
        /// </summary>
        [XmlIgnore]
        string ICriteria.BatchNumber { get { return TextConverter.UnescapeInvalidXmlChars(this.BatchNumber); } }

        /// <summary>
        /// Gets the optional additional flag whether all requested articles have to belong to one batch number.
        /// </summary>
        [XmlIgnore]
        bool ICriteria.SingleBatchNumber { get { return TypeConverter.ConvertBool(this.SingleBatchNumber); } }

        /// <summary>
        /// Gets the optional additional external identifier filter criterion for the requested packs.
        /// </summary>
        [XmlIgnore]
        string ICriteria.ExternalId { get { return TextConverter.UnescapeInvalidXmlChars(this.ExternalId); } }

        /// <summary>
        /// Gets the optional additional filter criterion to request only packs that have at least the specified expiry date.
        /// A value of null indicates that the expiry date is not relevant for the pack choice.
        /// </summary>
        [XmlIgnore]
        Nullable<DateTime> ICriteria.MinimumExpiryDate
        {
            get 
            {
                if (string.IsNullOrEmpty(this.MinimumExpiryDate))
                {
                    return null;
                }

                return TypeConverter.ConvertDate(this.MinimumExpiryDate);
            }
        }
        
        /// <summary>
        /// Gets the optional additional filter criterion which refers to the storage system internal pack identifier.
        /// </summary>
        [XmlIgnore]
        ulong ICriteria.PackId { get { return TypeConverter.ConvertULong(this.PackId); } }

        /// <summary>
        /// Gets the optional stock location filter criterion for the requested packs.
        /// </summary>
        [XmlIgnore]
        string ICriteria.StockLocationId
        {
            get { return this.StockLocationId != null ? TextConverter.EscapeInvalidXmlChars(this.StockLocationId) : string.Empty; }
        }

        /// <summary>
        /// Gets the optional machine location filter criterion for the requested packs.
        /// </summary>
        [XmlIgnore]
        string ICriteria.MachineLocation
        {
            get { return this.MachineLocation != null ? TextConverter.EscapeInvalidXmlChars(this.MachineLocation) : string.Empty; }
        }
        
        /// <summary>
        /// Gets the amount of full packs to dispense.
        /// </summary>
        [XmlIgnore]
        uint ICriteria.Quantity { get { return TypeConverter.ConvertUInt(this.Quantity); } }

        /// <summary>
        /// Gets the number of elements (e.g. pills or ampoules) to dispense. 
        /// If this property is set to a value greater than 0, the Quantity property is ignored and should be 0.
        /// </summary>
        [XmlIgnore]
        uint ICriteria.SubItemQuantity { get { return TypeConverter.ConvertUInt(this.SubItemQuantity); } }

        /// <summary>
        /// Gets the list of labels which are currently assigned to this output process criteria.
        /// </summary>
        [XmlIgnore]
        ILabel[] ICriteria.Labels 
        {
            get { return (this.Label != null) ? this.Label.ToArray() : new ILabel[0]; }
        }

        #endregion

        #region IShoppingCartCriteria Specific Properties

        /// <summary>
        /// Gets the unique Id of the shopping cart.
        /// </summary>
        [XmlIgnore]
        string IShoppingCartCriteria.ShoppingCartId { get { return TextConverter.UnescapeInvalidXmlChars(this.ShoppingCartId); } }

        /// <summary>
        /// Gets the unique Id of the sales point the shopping cart is currently assigned to.
        /// </summary>
        [XmlIgnore]
        string IShoppingCartCriteria.SalesPointId { get { return TextConverter.UnescapeInvalidXmlChars(this.SalesPointId); } }

        /// <summary>
        /// Gets the unique Id of the view point the shopping cart is currently being processed on.
        /// </summary>
        [XmlIgnore]
        string IShoppingCartCriteria.ViewPointId { get { return TextConverter.UnescapeInvalidXmlChars(this.ViewPointId); } }

        /// <summary>
        /// Gets the unique Id of the sales person that the shopping cart is currently assigned to.
        /// </summary>
        [XmlIgnore]
        string IShoppingCartCriteria.SalesPersonId { get { return TextConverter.UnescapeInvalidXmlChars(this.SalesPersonId); } }

        /// <summary>
        /// Gets the unique Id of the customer who the shopping cart belongs to.
        /// </summary>
        [XmlIgnore]
        string IShoppingCartCriteria.CustomerId { get { return TextConverter.UnescapeInvalidXmlChars(this.CustomerId); } }

        #endregion

        #region ICriteria Specific Methods

        /// <summary>
        /// Adds a new label definition to this output process criteria.
        /// </summary>
        /// <param name="templateId">
        /// The identifier of the label template which has to be used by 
        /// the label printer to process the label content correctly.
        /// </param>
        /// <param name="content">
        /// The arbitrary label content to use when printing labels on the packs.
        /// </param>
        void ICriteria.AddLabel(string templateId, string content)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                throw new ArgumentException("Invalid templateId specified.");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("Invalid content specified.");
            }

            if (this.Label == null)
            {
                this.Label = new List<Label>();
            }

            foreach (ILabel label in this.Label)
            {
                if (string.Compare(label.TemplateId, templateId) == 0)
                {
                    var errorMessage = string.Format("A label with the template identifier '{0}' already exists.", templateId);
                    throw new ArgumentException(errorMessage);
                }
            }

            this.Label.Add(new Label()
            {
                TemplateId = TextConverter.EscapeInvalidXmlChars(templateId),
                RawContent = content
            });
        }

        #endregion
    }
}
