using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 StockDeliverySetResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class StockDeliverySetResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public StockDeliverySetResponse StockDeliverySetResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 StockDeliverySetResponse message.
    /// </summary>
    public class StockDeliverySetResponse : MessageBase
    {
        #region Properties

        [XmlElement]
        public SetResult SetResult { get; set; }

        #endregion
    }
}
