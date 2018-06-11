using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.ArticleInformation
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ArticlePriceResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ArticlePriceResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ArticlePriceResponse ArticlePriceResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ArticlePriceResponse message.
    /// </summary>
    public class ArticlePriceResponse : MessageBase
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Currency { get; set; }

        [XmlElement]
        public Article[] Article { get; set; }

        #endregion
    }
}
