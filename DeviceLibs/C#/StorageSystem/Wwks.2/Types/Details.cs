using CareFusion.Lib.StorageSystem.Output;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Details datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Details
    {
        #region Properties

        [XmlAttribute]
        public string Priority { get; set; }

        [XmlAttribute]
        public string OutputDestination { get; set; }

        [XmlAttribute]
        public string OutputPoint { get; set; }

        [XmlAttribute]
        public string InputSource { get; set; }

        [XmlAttribute]
        public string InputPoint { get; set; }
        
        [XmlAttribute]
        public string Status { get; set; }

        #endregion
    }
}
