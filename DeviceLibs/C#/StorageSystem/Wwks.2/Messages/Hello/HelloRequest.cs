using CareFusion.Lib.StorageSystem.Wwks2.Types;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Hello
{
    /// <summary>
    /// Class which represents the WWKS 2.0 HelloRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class HelloRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public HelloRequest HelloRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 HelloRequest message.
    /// </summary>
    public class HelloRequest
    {
        #region Properties

        [XmlAttribute]
        public string Id { get; set; }

        [XmlElement]
        public Subscriber Subscriber { get; set; }        

        #endregion
    }
}
