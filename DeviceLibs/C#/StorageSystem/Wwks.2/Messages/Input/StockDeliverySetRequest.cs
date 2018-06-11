using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 StockDeliverySetRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class StockDeliverySetRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public StockDeliverySetRequest StockDeliverySetRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 StockDeliverySetRequest message.
    /// </summary>
    public class StockDeliverySetRequest : MessageBase
    {
        #region Properties

        [XmlElement]
        public StockDelivery[] StockDelivery { get; set; }

        #endregion
    }
}
