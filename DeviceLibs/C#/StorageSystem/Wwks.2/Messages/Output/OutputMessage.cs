using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Output
{
    /// <summary>
    /// Class which represents the WWKS 2.0 OutputMessage message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class OutputMessageEnvelope : EnvelopeBase
    {
        [XmlElement]
        public OutputMessage OutputMessage { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 OutputMessage message.
    /// </summary>
    public class OutputMessage : MessageBase, IOutputProcessInfo
    {
        #region WWKS 2.0 Properties

        [XmlElement]
        public Details Details { get; set; }

        [XmlElement]
        public Box[] Box { get; set; }

        [XmlElement]
        public Article[] Article { get; set; }

        #endregion

        #region IOutputProcessInfo Specific Properties

        /// <summary>
        /// Gets the current state of the output process.
        /// </summary>
        [XmlIgnore]
        OutputProcessState IOutputProcessInfo.State
        {
            get
            {
                if (this.Details == null)
                    return OutputProcessState.Unknown;

                return TypeConverter.ConvertEnum<OutputProcessState>(this.Details.Status, OutputProcessState.Unknown);
            }
        }

        /// <summary>
        /// Gets the list of packs which were dispensed by the output process.
        /// This property is set after the output process finished.
        /// </summary>
        [XmlIgnore]
        IDispensedPack[] IOutputProcessInfo.Packs
        {
            get
            {
                var dispensedPacks = new List<IDispensedPack>();

                if (this.Article != null)
                {
                    foreach (var article in this.Article)
                    {
                        if (article.Pack == null)
                            continue;

                        foreach (var pack in article.Pack)
                            dispensedPacks.Add(pack);
                    }
                }                

                return dispensedPacks.ToArray();
            }
        }

        /// <summary>
        /// Gets the list of boxes which were involved during pack dispensing.
        /// This property is set after the output process finished.
        /// </summary>
        [XmlIgnore]
        string[] IOutputProcessInfo.Boxes
        {
            get
            {
                var boxes = new List<string>();

                if (this.Box != null)
                {
                    foreach (var box in this.Box)
                        boxes.Add(box.Number);
                }

                return boxes.ToArray();
            }
        }

        #endregion
    }
}
