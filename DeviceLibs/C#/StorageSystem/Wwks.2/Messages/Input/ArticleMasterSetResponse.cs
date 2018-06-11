using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleMasterSetResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class ArticleMasterSetResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public ArticleMasterSetResponse ArticleMasterSetResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 ArticleMasterSetResponse message.
    /// </summary>
    public class ArticleMasterSetResponse : MessageBase
    {
        #region Properties

        [XmlElement]
        public SetResult SetResult { get; set; }

        #endregion
    }
}
