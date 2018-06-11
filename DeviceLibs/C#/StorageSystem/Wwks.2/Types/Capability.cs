using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Capability datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Capability
    {
        [XmlAttribute]
        public string Name { get; set; }
    }
}
