using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml.Serialization;
using System;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Sales
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ShoppingCartRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ShoppingCartRequest ShoppingCartRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartRequest message.
    /// </summary>
    public class ShoppingCartRequest : MessageBase, IShoppingCartRequest
    {
        #region Members

        /// <summary>
        /// Flag whether this shopping cart update request is finished.
        /// </summary>
        private bool _isFinished;

        #endregion

        #region WWKS 2.0 Properties

        [XmlElement]
        public Criteria Criteria { get; set; }

        #endregion

        #region Additional Properties

        /// <summary>
        /// Gets or sets the message object stream which is used to send the shopping cart response.
        /// </summary>
        [XmlIgnore]
        public XmlObjectStream MessageObjectStream { get; set; }

        #endregion

        #region IShoppingCartRequest Specific Properties

        /// <summary>
        /// Gets the criteria for the shopping cart.
        /// </summary>
        IShoppingCartCriteria IShoppingCartRequest.Criteria
        {
            get
            {
                return this.Criteria;
            }
        }

        #endregion

        #region IShoppingCartRequest Specific Methods

        /// <summary>
        /// Accepts the shopping cart request and sends the corresponding shopping cart response.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart that matches the critieria specified in the shopping cart request.</param>
        void IShoppingCartRequest.Accept(IShoppingCart shoppingCart)
        {
            SendResponse(shoppingCart);
        }

        /// <summary>
        /// Rejects the udpate request.
        /// No shopping cart will be sent to the digital shelf.
        /// </summary>
        void IShoppingCartRequest.Reject()
        {
            SendResponse(null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sends the corresponding shopping cart response to the digital shelf.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart that matches the critieria specified in the shopping cart request.</param>
        private void SendResponse(IShoppingCart shoppingCart)
        {
            if (!_isFinished)
            {
                var response = new ShoppingCartResponseEnvelope()
                {
                    ShoppingCartResponse = new ShoppingCartResponse()
                    {
                        Id = this.Id,
                        Source = this.Destination,
                        Destination = this.Source,
                        ShoppingCart = (shoppingCart != null) ? (ShoppingCart)shoppingCart : null
                    }
                };

                if (!this.MessageObjectStream.Write(response))
                {
                    throw new ApplicationException("Sending 'ShoppingCartResponse' to digital shelf failed.");
                }

                _isFinished = true;
            }
        }

        #endregion
    }
}
