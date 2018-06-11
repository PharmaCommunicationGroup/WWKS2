using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Stock
{
    /// <summary>
    /// Class which represents the WWKS 2.0 StockLocationInfoRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class StockLocationInfoRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public StockLocationInfoRequest StockLocationInfoRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 StockLocationInfoRequest message.
    /// </summary>
    public class StockLocationInfoRequest : MessageBase
    {
    }
}
