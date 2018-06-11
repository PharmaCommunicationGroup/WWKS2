using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf
{
    public class AlternativeArticles
    {
        [XmlElement]
        public List<Article> Article { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativeArticles"/> class.
        /// </summary>
        public AlternativeArticles()
        {
            Article = new List<Article>();
        }
    }
}
