using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Box datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Box
    {
        [XmlAttribute]
        public string Number { get; set; }
    }
}
