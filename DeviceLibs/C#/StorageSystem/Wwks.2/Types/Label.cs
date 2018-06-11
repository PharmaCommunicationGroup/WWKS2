using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.Xml;
using System.Xml;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Class which represents the Label datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Label : ILabel
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string TemplateId { get; set; }

        [XmlIgnore]
        public string RawContent { get; set; }

        [XmlElement(ElementName = "Content")]
        public XmlCDataSection XmlContent
        {
            get
            {
                if (string.IsNullOrEmpty(this.RawContent))
                {
                    return null;
                }

                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(this.RawContent);
            }

            set
            {
                if (value == null)
                {
                    RawContent = string.Empty;
                }
                else
                {
                    RawContent = value.Value;
                }
            }
        }

        #endregion

        #region ILabel Specific Properties

        /// <summary>
        /// Gets the identifier of the label template which has to be used by 
        /// the label printer to process the label content correctly.
        /// </summary>
        [XmlIgnore]
        string ILabel.TemplateId
        {
            get { return TextConverter.UnescapeInvalidXmlChars(this.TemplateId); }
        }

        /// <summary>
        /// Gets the arbitrary label content to use when printing labels on the packs.
        /// </summary>
        [XmlIgnore]
        string ILabel.Content
        {
            get { return this.RawContent; }
        }

        #endregion        
    }
}
