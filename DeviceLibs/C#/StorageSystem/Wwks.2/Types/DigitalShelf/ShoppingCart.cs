using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf
{
    /// <summary>
    /// Class which represents the ShoppingCart datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class ShoppingCart : IShoppingCart
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Status { get; set; }

        [XmlAttribute]
        public string ViewPointId { get; set; }

        [XmlAttribute]
        public string SalesPointId { get; set; }

        [XmlAttribute]
        public string SalesPersonId { get; set; }

        [XmlAttribute]
        public string CustomerId { get; set; }

        [XmlElement]
        public List<ShoppingCartItem> ShoppingCartItem { get; set; }

        #endregion

        #region IShoppingCart Specific Properties

        /// <summary>
        /// Gets the unique Id of the shopping cart.
        /// </summary>
        [XmlIgnore]
        string IShoppingCart.Id
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.Id);
            }
        }

        /// <summary>
        /// Gets the current status of the shopping cart.
        /// </summary>
        [XmlIgnore]
        ShoppingCartStatus IShoppingCart.Status
        {
            get
            {
                return TypeConverter.ConvertEnum<ShoppingCartStatus>(this.Status, ShoppingCartStatus.Active);
            }
            set
            {
                this.Status = value.ToString();
            }
        }

        /// <summary>
        /// Gets the unique Id of the sales point the shopping cart is currently assigned to.
        /// </summary>
        [XmlIgnore]
        string IShoppingCart.SalesPointId
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.SalesPointId);
            }
            set
            {
                this.SalesPointId = TextConverter.EscapeInvalidXmlChars(value);
            }
        }

        /// <summary>
        /// Gets the unique Id of the view point the shopping cart is currently being processed on.
        /// </summary>
        [XmlIgnore]
        string IShoppingCart.ViewPointId
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.ViewPointId);
            }
            set
            {
                this.ViewPointId = TextConverter.EscapeInvalidXmlChars(value);
            }
        }

        /// <summary>
        /// Gets the unique Id of the sales person that the shopping cart is currently assigned to.
        /// </summary>
        [XmlIgnore]
        string IShoppingCart.SalesPersonId
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.SalesPersonId);
            }
            set
            {
                this.SalesPersonId = TextConverter.EscapeInvalidXmlChars(value);
            }
        }

        /// <summary>
        /// Gets the unique Id of the customer who the shopping cart belongs to.
        /// </summary>
        [XmlIgnore]
        string IShoppingCart.CustomerId
        {
            get
            {
                return TextConverter.UnescapeInvalidXmlChars(this.CustomerId);
            }
            set
            {
                this.CustomerId = TextConverter.EscapeInvalidXmlChars(value);
            }
        }

        /// <summary>
        /// Gets the detailed information of the items the shopping cart currently contains.
        /// </summary>
        [XmlIgnore]
        IShoppingCartItem[] IShoppingCart.ShoppingCartItems
        {
            get
            {
                return (this.ShoppingCartItem != null) ? (IShoppingCartItem[])this.ShoppingCartItem.ToArray() : new IShoppingCartItem[0];
            }
        }

        #endregion

        #region IShoppingCart Specific Methods

        /// <summary>
        /// Adds an item to the shopping cart.
        /// </summary>
        /// <param name="articleId">The unique article Id of the shopping cart item.</param>
        /// <param name="orderedQuantity">The total amount of packs of this article in the shopping cart.</param>
        /// <param name="dispensedQuantity">The amount of packs that has already been dispensed from a storage system.</param>
        /// <param name="paidQuantity">The amount of packs that have been paid.</param>
        /// <param name="price">The total price specified in the smallest unit of the currency (e.g. Cent).</param>
        /// <param name="currency">The ISO 4217 currency symbol in which the price is specified (e.g. EUR).</param>
        void IShoppingCart.AddItem(string articleId, uint orderedQuantity, uint dispensedQuantity, uint paidQuantity, string price, string currency)
        {
            if (this.ShoppingCartItem == null)
            {
                this.ShoppingCartItem = new List<ShoppingCartItem>();
            }

            var shoppingCartItem = new ShoppingCartItem()
            {
                ArticleId = TextConverter.EscapeInvalidXmlChars(articleId),
                OrderedQuantity = orderedQuantity.ToString(),
                DispensedQuantity = dispensedQuantity.ToString(),
                PaidQuantity = paidQuantity.ToString(),
                Price = TextConverter.EscapeInvalidXmlChars(price),
                Currency = TextConverter.EscapeInvalidXmlChars(currency)
            };

            this.ShoppingCartItem.Add(shoppingCartItem);
        }

        #endregion
    }
}
