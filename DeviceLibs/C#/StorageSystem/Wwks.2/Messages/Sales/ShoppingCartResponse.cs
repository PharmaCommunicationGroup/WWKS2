using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Sales
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ShoppingCartResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ShoppingCartResponse ShoppingCartResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartResponse message.
    /// </summary>
    public class ShoppingCartResponse : MessageBase
    {
        [XmlElement]
        public ShoppingCart ShoppingCart { get; set; }
    }
}
