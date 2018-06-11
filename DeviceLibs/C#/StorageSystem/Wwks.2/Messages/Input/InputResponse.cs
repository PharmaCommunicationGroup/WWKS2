using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 InputResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class InputResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public InputResponse InputResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 InputResponse message.
    /// </summary>
    public class InputResponse : MessageBase
    {
        #region Properties

        [XmlAttribute]
        public string IsNewDelivery { get; set; }

        [XmlElement]
        public List<Article> Article { get; set; }

        #endregion
    }
}
