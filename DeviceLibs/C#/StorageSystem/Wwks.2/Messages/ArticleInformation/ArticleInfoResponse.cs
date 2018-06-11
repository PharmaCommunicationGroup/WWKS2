using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.ArticleInformation
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleInfoResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ArticleInfoResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ArticleInfoResponse ArticleInfoResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleInfoResponse message.
    /// </summary>
    public class ArticleInfoResponse : MessageBase
    {
        #region WWKS 2.0 Properties

        [XmlElement]
        public Article[] Article { get; set; }

        #endregion
    }
}
