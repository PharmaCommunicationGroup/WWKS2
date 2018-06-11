using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Types
{
    /// <summary>
    /// Enum which defines the different supported task types.
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// The task is a stock output process.
        /// </summary>
        Output,

        /// <summary>
        /// The task is a stock delivery process.
        /// </summary>
        StockDelivery
    }

    /// <summary>
    /// Class which represents the Task datatype in the WWKS 2.0 protocol.
    /// </summary>
    public class Task : IOutputProcessInfo, IStockDeliveryInfo
    {
        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public TaskType Type { get; set; }

        [XmlAttribute]
        public string Status { get; set; }

        [XmlElement]
        public Article[] Article { get; set; }

        [XmlElement]
        public Box[] Box { get; set; }

        #endregion

        #region IOutputProcessInfo Specific Properties

        [XmlIgnore]
        OutputProcessState IOutputProcessInfo.State
        {
            get { return TypeConverter.ConvertEnum<OutputProcessState>(this.Status, OutputProcessState.Unknown); }
        }

        [XmlIgnore]
        IDispensedPack[] IOutputProcessInfo.Packs
        {
            get 
            {
                var packs = new List<IDispensedPack>();

                if (this.Article != null)
                {
                    foreach (var article in this.Article)
                    {
                        if (article.Pack == null)
                        {
                            continue;
                        }

                        foreach (var pack in article.Pack)
                        {
                            pack.ArticleId = TextConverter.UnescapeInvalidXmlChars(article.Id);
                        }

                        packs.AddRange(article.Pack);
                    }
                }

                return packs.ToArray();
            }
        }

        [XmlIgnore]
        string[] IOutputProcessInfo.Boxes
        {
            get 
            {
                var boxes = new List<string>();

                if (this.Box != null)
                {
                    foreach (var box in this.Box)
                    {
                        boxes.Add(box.Number);
                    }
                }

                return boxes.ToArray();
            }
        }

        #endregion

        #region IStockDeliveryInfo Specific Properties

        /// <summary>
        /// Gets the delivery number of the stock delivery.
        /// </summary>
        [XmlIgnore]
        string IStockDeliveryInfo.DeliveryNumber
        {
            get { return this.Id; }
        }

        /// <summary>
        /// Gets the current state of the output process.
        /// </summary>
        [XmlIgnore]
        StockDeliveryState IStockDeliveryInfo.State
        {
            get { return TypeConverter.ConvertEnum<StockDeliveryState>(this.Status, StockDeliveryState.Unknown); }
        }

        /// <summary>
        /// Gets the information of the articles and the packs that were already
        /// stored in the storage system during the processing of this stock delivery.
        /// </summary>
        [XmlIgnore]
        Stock.IArticle[] IStockDeliveryInfo.InputArticles
        {
            get { return this.Article != null ? this.Article : new Stock.IArticle[0]; }
        }

        #endregion        
    }
}
