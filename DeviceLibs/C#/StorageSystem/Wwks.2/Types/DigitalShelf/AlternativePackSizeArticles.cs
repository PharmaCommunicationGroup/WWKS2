using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf
{
    public class AlternativePackSizeArticles
    {
        [XmlElement]
        public List<Article> Article { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativePackSizeArticles"/> class.
        /// </summary>
        public AlternativePackSizeArticles()
        {
            Article = new List<Article>();
        }
    }
}
