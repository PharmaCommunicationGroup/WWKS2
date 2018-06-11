using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Enum which defines the different types of supported subscribers.
    /// </summary>
    public enum SubscriberType
    {
        IMS,
        Robot
    }

    /// <summary>
    /// Class which represents the Subscriber datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Subscriber
    {
        #region Properties

        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public SubscriberType Type { get; set; }

        [XmlAttribute]
        public string Manufacturer { get; set; }

        [XmlAttribute]
        public string ProductInfo { get; set; }

        [XmlAttribute]
        public string VersionInfo { get; set; }

        [XmlAttribute]
        public string TenantId { get; set; }

        [XmlElement]
        public Capability[] Capability { get; set; }

        #endregion
    }
}
