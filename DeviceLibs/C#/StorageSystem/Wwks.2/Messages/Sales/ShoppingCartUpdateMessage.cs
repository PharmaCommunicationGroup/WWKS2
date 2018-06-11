using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Sales
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartUpdateMessage message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ShoppingCartUpdateMessageEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ShoppingCartUpdateMessage ShoppingCartUpdateMessage { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartUpdateMessage message.
    /// </summary>
    public class ShoppingCartUpdateMessage : MessageBase
    {
        [XmlElement]
        public ShoppingCart ShoppingCart { get; set; }
    }
}
