using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf
{
    /// <summary>
    /// Class which represents the ShoppingCartItem datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class ShoppingCartItem : IShoppingCartItem
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string ArticleId { get; set; }

        [XmlAttribute]
        public string OrderedQuantity { get; set; }

        [XmlAttribute]
        public string DispensedQuantity { get; set; }

        [XmlAttribute]
        public string PaidQuantity { get; set; }

        [XmlAttribute]
        public string Price { get; set; }

        [XmlAttribute]
        public string Currency { get; set; }

        #endregion

        #region IShoppingCartItem Specific Properties

        /// <summary>
        /// Gets the unique article Id of the shopping cart item.
        /// </summary>
        [XmlIgnore]
        string IShoppingCartItem.ArticleId
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.ArticleId);
            }
        }

        /// <summary>
        /// Gets the total amount of packs of this article in the shopping cart.
        /// </summary>
        [XmlIgnore]
        uint IShoppingCartItem.OrderedQuantity
        {
            get
            {
                return TypeConverter.ConvertUInt(this.OrderedQuantity);
            }
        }

        /// <summary>
        /// Gets the amount of packs that has already been dispensed from a storage system.
        /// </summary>
        [XmlIgnore]
        uint IShoppingCartItem.DispensedQuantity
        {
            get
            {
                return TypeConverter.ConvertUInt(this.DispensedQuantity);
            }
        }

        /// <summary>
        /// Gets the amount of packs that have been paid.
        /// </summary>
        [XmlIgnore]
        uint IShoppingCartItem.PaidQuantity
        {
            get
            {
                return TypeConverter.ConvertUInt(this.PaidQuantity);
            }
        }

        /// <summary>
        /// Gets the total price specified in the smallest unit of the currency (e.g. Cent).
        /// </summary>
        [XmlIgnore]
        decimal IShoppingCartItem.Price
        {
            get
            {
                return TypeConverter.ConvertDecimal(this.Price);
            }
        }

        /// <summary>
        /// Gets the ISO 4217 currency symbol in which the price is specified (e.g. EUR).
        /// </summary>
        [XmlIgnore]
        string IShoppingCartItem.Currency
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.Currency);
            }
        }

        #endregion
    }
}
