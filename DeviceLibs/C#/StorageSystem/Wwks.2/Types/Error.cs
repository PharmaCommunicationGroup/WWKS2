using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Box datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Error 
    {
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string Text { get; set; }
    }
}
