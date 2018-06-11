using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Sales
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleSelectedMessage message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ArticleSelectedMessageEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ArticleSelectedMessage ArticleSelectedMessage { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleSelectedMessage message.
    /// </summary>
    public class ArticleSelectedMessage : MessageBase
    {
        [XmlElement]
        public Article Article { get; set; }
    }
}
