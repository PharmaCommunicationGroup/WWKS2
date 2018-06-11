using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf
{
    /// <summary>
    /// Class which represents the PriceInformation datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class PriceInformation : IPriceInformation
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Category { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string Quantity { get; set; }

        [XmlAttribute]
        public string Price { get; set; }

        [XmlAttribute]
        public string BasePrice { get; set; }

        [XmlAttribute]
        public string BasePriceUnit { get; set; }

        [XmlAttribute]
        public string VAT { get; set; }

        #endregion

        #region IPriceInformation Specific Properties

        /// <summary>
        /// Gets the category of the price information (e.g. "RRP" or "Offer").
        /// </summary>
        [XmlIgnore]
        PriceCategory IPriceInformation.Category
        {
            get
            {
                return TypeConverter.ConvertEnum<PriceCategory>(this.Category, PriceCategory.Other);
            }
        }

        /// <summary>
        /// Gets the description of the price information (e.g. the name of a special offer).
        /// </summary>
        [XmlIgnore]
        string IPriceInformation.Description
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.Description);
            }
        }

        /// <summary>
        /// Gets the quantity for which the price is valid (e.g "5" if this price is valid if the customer buys at least 5 packs of the article).
        /// </summary>
        [XmlIgnore]
        uint IPriceInformation.Quantity
        {
            get
            {
                return TypeConverter.ConvertUInt(this.Quantity);
            }
        }

        /// <summary>
        /// Gets the actual price specified in the smallest unit of the specified currency
        /// </summary>
        [XmlIgnore]
        decimal IPriceInformation.Price
        {
            get
            {
                return TypeConverter.ConvertDecimal(this.Price);
            }
        }

        /// <summary>
        /// Gets the base price specified in the smallest unit of the specified currency
        /// </summary>
        [XmlIgnore]
        decimal IPriceInformation.BasePrice
        {
            get
            {
                return TypeConverter.ConvertDecimal(this.BasePrice);
            }
        }

        /// <summary>
        /// Gets the base price unit for the specified base price
        /// </summary>
        [XmlIgnore]
        string IPriceInformation.BasePriceUnit
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.BasePriceUnit);
            }
        }

        /// <summary>
        /// Gets the VAT for the specified base price.
        /// </summary>
        [XmlIgnore]
        string IPriceInformation.VAT
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.VAT);
            }
        }

        #endregion
    }
}
