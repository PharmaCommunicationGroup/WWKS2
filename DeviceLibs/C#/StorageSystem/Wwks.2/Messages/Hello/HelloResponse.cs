using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Hello
{
    /// <summary>
    /// Class which represents the WWKS 2.0 HelloResponse message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class HelloResponseEnvelope : EnvelopeBase
    {
        [XmlElement]
        public HelloResponse HelloResponse { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 HelloResponse message.
    /// </summary>
    public class HelloResponse
    {
        #region Properties

        [XmlAttribute]
        public string Id { get; set; }

        [XmlElement]
        public Subscriber Subscriber { get; set; }

        #endregion
    }
}
