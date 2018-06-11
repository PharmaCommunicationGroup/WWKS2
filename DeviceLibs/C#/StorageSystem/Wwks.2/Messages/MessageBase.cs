using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages
{
    /// <summary>
    /// Class which defines the basic attributes of every WWKS 2.0 message.
    /// </summary>
    public class MessageBase
    {
        #region Properties

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public int Source { get; set; }

        [XmlAttribute]
        public int Destination { get; set; }

        #endregion
    }
}
