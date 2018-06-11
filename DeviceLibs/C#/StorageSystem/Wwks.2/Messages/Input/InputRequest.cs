using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Input
{
    /// <summary>
    /// Class which represents the WWKS 2.0 InputRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class InputRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public InputRequest InputRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 InputRequest message.
    /// </summary>
    public class InputRequest : MessageBase, IInputRequest
    {
        #region Members

        /// <summary>
        /// Flag whether this input request is finished.
        /// </summary>
        private bool _isFinished;

        #endregion

        #region WWKS 2.0 Properties

        [XmlAttribute]
        public string IsNewDelivery { get; set; }

        [XmlAttribute]
        public string SetPickingIndicator { get; set; }

        [XmlElement]
        public Details Details { get; set; }

        [XmlElement]
        public Article Article { get; set; }

        #endregion

        #region IInputRequest Specific Properties

        /// <summary>
        /// Gets the identifier of the input request.
        /// </summary>
        [XmlIgnore]
        int IInputRequest.Id
        {
            get { return TypeConverter.ConvertInt(this.Id); }
        }

        /// <summary>
        /// Gets the source identifier of the input request.
        /// </summary>
        [XmlIgnore]
        int IInputRequest.Source
        {
            get { return this.Source; }
        }

        /// <summary>
        /// If the pack input is part of a new stock delivery, this property
        /// specifies the delivery number which is usually entered by the user
        /// at the storage system UI.
        /// If this property is null, this pack input is NOT part of a new delivery.
        /// </summary>
        [XmlIgnore]
        string IInputRequest.DeliveryNumber
        {
            get
            {
                if (TypeConverter.ConvertBool(this.IsNewDelivery) == false)
                {
                    return null;
                }

                string result = string.Empty;

                if ((this.Article != null) && (this.Article.Pack != null))
                {
                    foreach (var pack in this.Article.Pack)
                    {
                        if (string.IsNullOrEmpty(pack.DeliveryNumber) == false)
                        {
                            result = pack.DeliveryNumber;
                            break;
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Flag whether a so-called picking indicator was set by the 
        /// storage system to enforce a redefinition of the requested 
        /// pack articles as storage system capable articles.
        /// This flag is usually used when the new storage system 
        /// gets filled for the very first time to realize a kind
        /// of first-time synchronization between the IT system and 
        /// the storage system according to which articles are "storage
        /// system articles".
        /// </summary>
        [XmlIgnore]
        bool IInputRequest.PickingIndicator { get { return TypeConverter.ConvertBool(this.SetPickingIndicator); } }

        /// <summary>
        /// Gets the list of packs, the storage system wants to input.
        /// The IT system iterates this pack list, defines further article details
        /// for each pack and decides wether to allow the pack input or not.
        /// Afterwards the method "Finish" is called to complete the input process.
        /// </summary>
        [XmlIgnore]
        IInputPack[] IInputRequest.Packs
        {
            get
            {
                if ((this.Article != null) && (this.Article.Pack != null))
                {
                    return (IInputPack[])this.Article.Pack.ToArray();
                }

                return new IInputPack[0];
            }
        }

        #endregion

        #region Additional Properties

        /// <summary>
        /// Gets or sets the message object stream which is used to send the InputResponse.
        /// </summary>
        [XmlIgnore]
        public XmlObjectStream MessageObjectStream { get; set; }

        #endregion

        #region IInputRequest Specific Methods

        /// <summary>
        /// Finishes the input request by sending the according answer to
        /// the storage system. This method has to be called after the list of
        /// requested packs was updated to allow or reject pack input.
        /// </summary>
        void IInputRequest.Finish()
        {
            if ((_isFinished) || 
                (this.Article == null) || 
                (this.Article.Pack == null))
            {
                return;
            }

            if (this.MessageObjectStream == null)
            {
                throw new ApplicationException("No storage system connection available.");
            }

            var articleMap = new Dictionary<string, Article>();
            var articleChildMap = new Dictionary<string, Article>();

            foreach (var pack in this.Article.Pack)
            {
                if (pack.ParentArticle != null)
                {
                    if (articleMap.ContainsKey(pack.ParentArticle.Id) == false)
                    {
                        articleMap.Add(pack.ParentArticle.Id, pack.ParentArticle);
                    }

                    if (articleMap[pack.ParentArticle.Id].ChildArticle == null)
                    {
                        articleMap[pack.ParentArticle.Id].ChildArticle = new List<Article>();
                    }

                    if (pack.Article != null)
                    {
                        if (!articleChildMap.ContainsKey(pack.Article.Id))
                        {
                            pack.Article.Pack = new List<Pack>();
                            articleMap[pack.ParentArticle.Id].ChildArticle.Add(pack.Article);
                            articleChildMap.Add(pack.Article.Id, pack.Article);
                        }
                        articleChildMap[pack.Article.Id].Pack.Add(pack);
                    }
                    
                    if (pack.OtherArticles != null)
                    {
                        pack.ParentArticle.ChildArticle.AddRange(pack.OtherArticles);
                        foreach (var childArticle in pack.OtherArticles)
                        {
                            if (!articleChildMap.ContainsKey(childArticle.Id))
                            {
                                articleChildMap.Add(childArticle.Id, childArticle);
                            }
                        }
                    }
                }
                else
                {
                    if (pack.Article == null)
                    {
                        if (articleMap.ContainsKey(pack.ScanCode) == false)
                        {
                            articleMap.Add(pack.ScanCode, new Article()
                            {
                                Id = null,
                                Name = null,
                                DosageForm = null,
                                PackagingUnit = null,
                                Pack = new List<Pack>()
                            });
                        }

                        articleMap[pack.ScanCode].Pack.Add(pack);
                        continue;
                    }

                    if (articleMap.ContainsKey(pack.Article.Id) == false)
                    {
                        pack.Article.Pack = new List<Pack>();
                        articleMap.Add(pack.Article.Id, pack.Article);
                    }
                    articleMap[pack.Article.Id].Pack.Add(pack);
                }
            }

            var response = new InputResponseEnvelope()
            {
                InputResponse = new InputResponse() 
                {
                    Id = this.Id,
                    Source = this.Destination,
                    Destination = this.Source,
                    IsNewDelivery = this.IsNewDelivery,
                    Article = new List<Article>(articleMap.Values)
                }
            };

            if (this.MessageObjectStream.Write(response) == false)
            {
                throw new ApplicationException("Sending 'InputResponse' to storage system failed.");
            }

            _isFinished = true;
        }

        #endregion
    }
}
