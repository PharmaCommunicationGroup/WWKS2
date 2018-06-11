using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Sales
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartUpdateResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ShoppingCartUpdateResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ShoppingCartUpdateResponse ShoppingCartUpdateResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ShoppingCartUpdateResponse message.
    /// </summary>
    public class ShoppingCartUpdateResponse : MessageBase
    {
        [XmlElement]
        public ShoppingCart ShoppingCart { get; set; }

        [XmlElement]
        public UpdateResult UpdateResult { get; set; }
    }
}
