using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleMasterSetRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ArticleMasterSetRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ArticleMasterSetRequest ArticleMasterSetRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleMasterSetRequest message.
    /// </summary>
    public class ArticleMasterSetRequest : MessageBase
    {
        #region Properties

        [XmlElement]
        public Article[] Article { get; set; }

        #endregion
    }
}
