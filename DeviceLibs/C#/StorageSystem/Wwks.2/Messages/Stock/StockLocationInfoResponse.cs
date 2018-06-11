using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Stock
{
    /// <summary>
    /// Class which represents the WWKS 2.0 StockLocationInfoResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class StockLocationInfoResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public StockLocationInfoResponse StockLocationInfoResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 StockLocationInfoResponse message.
    /// </summary>
    public class StockLocationInfoResponse : MessageBase
    {
        #region Properties

        [XmlElement]
        public StockLocation[] StockLocation { get; set; }

        #endregion
    }
}
