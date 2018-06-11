using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Sales
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartUpdateRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ShoppingCartUpdateRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ShoppingCartUpdateRequest ShoppingCartUpdateRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartUpdateRequest message.
    /// </summary>
    public class ShoppingCartUpdateRequest : MessageBase, IShoppingCartUpdateRequest
    {
        #region Members

        /// <summary>
        /// Flag whether this shopping cart update request is finished.
        /// </summary>
        private bool _isFinished;

        #endregion

        #region WWKS 2.0 Properties

        [XmlElement]
        public ShoppingCart ShoppingCart { get; set; }

        #endregion

        #region Additional Properties

        /// <summary>
        /// Gets or sets the message object stream which is used to send the shopping cart update response.
        /// </summary>
        [XmlIgnore]
        public XmlObjectStream MessageObjectStream { get; set; }

        #endregion

        #region IShoppingCartUpdateRequest Specific Methods

        /// <summary>
        /// Gets the shopping cart which is to be updated.
        /// </summary>
        IShoppingCart IShoppingCartUpdateRequest.ShoppingCart
        {
            get
            {
                return this.ShoppingCart;
            }
        }

        #endregion

        #region IShoppingCartUpdateRequest Specific Methods

        /// <summary>
        /// Finishes the shopping cart update request after by accepting all the requested changes.
        /// This is to be called after all the changes have been applied to the shopping cart in the It-System.
        /// </summary>
        /// <param name="description">The (optional) description to be sent with the shopping cart update response.</param>
        void IShoppingCartUpdateRequest.Accept(string description)
        {
            SendResponse(this.ShoppingCart, UpdateResultStatus.Updated, description);
        }

        /// <summary>
        /// Finishes the shopping cart update request after by rejecting all the requested changes.
        /// This means that none of the requested changes have been applied to the shopping cart in the It-System..
        /// </summary>
        /// <param name="shoppingCart">The actual shopping cart of the It-System in its current valid state.</param>
        /// <param name="description">The (optional) description to be sent with the shopping cart update response.</param>
        void IShoppingCartUpdateRequest.Reject(IShoppingCart shoppingCart, string description)
        {
            SendResponse(shoppingCart, UpdateResultStatus.NotUpdated, description);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sends the corresponding shopping cart update response to the digital shelf.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart in it's current state.</param>
        /// <param name="status">The status of the update.</param>
        /// <param name="description">The (optional) description.</param>
        private void SendResponse(IShoppingCart shoppingCart, UpdateResultStatus status, string description)
        {
            if (!_isFinished)
            {
                var response = new ShoppingCartUpdateResponseEnvelope()
                {
                    ShoppingCartUpdateResponse = new ShoppingCartUpdateResponse()
                    {
                        Id = this.Id,
                        Source = this.Destination,
                        Destination = this.Source,
                        ShoppingCart = (ShoppingCart)shoppingCart,
                        UpdateResult = new UpdateResult()
                        {
                            Status = status,
                            Description = description
                        }
                    }
                };

                if (!this.MessageObjectStream.Write(response))
                {
                    throw new ApplicationException("Sending 'ShoppingCartUpdateResponse' to digital shelf failed.");
                }

                _isFinished = true;
            }
        }

        #endregion
    }
}
