using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf
{
    public class CrossSellingArticles
    {
        [XmlElement]
        public List<Article> Article { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossSellingArticles"/> class.
        /// </summary>
        public CrossSellingArticles()
        {
            Article = new List<Article>();
        }
    }
}
