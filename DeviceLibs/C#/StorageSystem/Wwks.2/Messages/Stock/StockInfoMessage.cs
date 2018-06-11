using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Stock
{
    /// <summary>
    /// Class which represents the WWKS 2.0 StockInfoResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class StockInfoMessageEnvelope : EnvelopeBase
    {
        [XmlElement]
        public StockInfoMessage StockInfoMessage { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 StockInfoResponse message.
    /// </summary>
    public class StockInfoMessage : MessageBase
    {
        #region Properties

        [XmlElement]
        public Article[] Article { get; set; }

        #endregion
    }
}
