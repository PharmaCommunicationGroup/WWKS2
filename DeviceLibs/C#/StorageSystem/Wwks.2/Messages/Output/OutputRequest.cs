using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Task;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Xml;
using CareFusion.Lib.StorageSystem.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using System.Linq;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages.Output
{
    /// <summary>
    /// Class which represents the WWKS 2.0 OutputRequest message envelope.
    /// </summary>
    [XmlType(TypeName = "WWKS")]
    public class OutputRequestEnvelope : EnvelopeBase
    {
        [XmlElement]
        public OutputRequest OutputRequest { get; set; }
    }

    /// <summary>
    /// Class which represents the WWKS 2.0 OutputRequest message.
    /// </summary>
    public class OutputRequest : MessageBase, IOutputProcess
    {
        #region Members

        /// <summary>
        /// The message object stream to use for sending the requests.
        /// </summary>
        private XmlObjectStream _messageOjectStream;

        /// <summary>
        /// The message dispatcher to use for message interception.
        /// </summary>
        private MessageDispatcher _messageDispatcher;

        /// <summary>
        /// List of packs that where dispensed.
        /// </summary>
        private List<IDispensedPack> _dispensedPackList = new List<IDispensedPack>();

        /// <summary>
        /// List of boxes that were involved during dispensing.
        /// </summary>
        private HashSet<string> _boxList = new HashSet<string>();

        /// <summary>
        /// The current state of the output process.
        /// </summary>
        private OutputProcessState _currentState = OutputProcessState.Created;

        /// <summary>
        /// The reference to the internal finished event.
        /// </summary>
        private EventHandler _finished;

        /// <summary>
        /// The reference to the internal box released event.
        /// </summary>
        private EventHandler _boxReleased;

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private object _syncLock = new object();

        #endregion

        #region WWKS 2.0 Properties

        [XmlElement]
        public Details Details { get; set; }

        [XmlElement]
        public List<Criteria> Criteria { get; set; }

        [XmlAttribute]
        public string BoxNumber { get; set; }

        #endregion

        #region IOutputProcess Specific Properties

        /// <summary>
        /// Gets the optional box number which belongs to this output process.
        /// </summary>
        [XmlIgnore]
        string IOutputProcess.BoxNumber
        {
            get { return TextConverter.UnescapeInvalidXmlChars(this.BoxNumber); }
        }

        /// <summary>
        /// Gets the unique order number which is used to identify an output process.
        /// </summary>
        [XmlIgnore]
        string IOutputProcess.OrderNumber
        {
            get { return this.Id; }
        }

        /// <summary>
        /// Gets the current state of the output process.
        /// </summary>
        [XmlIgnore]
        OutputProcessState IOutputProcessInfo.State
        {
            get
            {
                lock (_syncLock)
                { 
                    if ((_currentState != OutputProcessState.Queued) &&
                        (_currentState != OutputProcessState.InProcess) &&
                        (_currentState != OutputProcessState.Aborting))
                    {
                        return _currentState;
                    }
                }

                var request = new TaskInfoRequestEnvelope()
                {
                    TaskInfoRequest = new TaskInfoRequest()
                    {
                        Id = MessageId.Next,
                        Destination = this.Destination,
                        Source = this.Source,
                        IncludeTaskDetails = false.ToString()
                    }
                };

                request.TaskInfoRequest.Task = new Types.Task[] 
                { 
                    new Types.Task() 
                    { 
                        Id = this.Id.ToString(), 
                        Type = TaskType.Output 
                    }
                };

                var response = (TaskInfoResponse)_messageDispatcher.SendAndWaitForResponse(request,
                                                                                           request.TaskInfoRequest.Id,
                                                                                           typeof(TaskInfoResponse));

                if ((response == null) ||
                    (response.Task == null) ||
                    (response.Task.Length == 0))
                {
                    return OutputProcessState.Unknown;
                }

                return TypeConverter.ConvertEnum<OutputProcessState>(response.Task[0].Status, OutputProcessState.Unknown);
            }
        }

        /// <summary>
        /// Gets the priority of the output process.
        /// </summary>
        [XmlIgnore]
        OutputProcessPriority IOutputProcess.Priority
        {
            get 
            { 
                if (this.Details == null)
                {
                    return OutputProcessPriority.Normal;
                }

                if (string.IsNullOrEmpty(this.Details.Priority))
                {
                    return OutputProcessPriority.Normal;
                }

                OutputProcessPriority result = OutputProcessPriority.Normal;
                Enum.TryParse<OutputProcessPriority>(this.Details.Priority, out result);
                return result; 
            }
        }

        /// <summary>
        /// Gets the output destination identifier for the output process.
        /// </summary>
        [XmlIgnore]
        int IOutputProcess.OutputDestination
        {
            get 
            { 
                if ((this.Details == null) || (string.IsNullOrEmpty(this.Details.OutputDestination)))
                {
                    return 0;
                }

                int destination = 0;

                if (int.TryParse(this.Details.OutputDestination, out destination))
                {
                    return destination;
                }

                return 0; 
            }
        }

        /// <summary>
        /// Gets the more detailed output destination point (e.g. belt number) for the output process.
        /// </summary>
        [XmlIgnore]
        int IOutputProcess.OutputPoint
        {
            get
            {
                if ((this.Details == null) || (string.IsNullOrEmpty(this.Details.OutputPoint)))
                {
                    return 0;
                }

                int point = 0;

                if (int.TryParse(this.Details.OutputPoint, out point))
                {
                    return point;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the list of defined output criteria.
        /// </summary>
        [XmlIgnore]
        ICriteria[] IOutputProcess.Criteria
        {
            get { return (this.Criteria != null) ? this.Criteria.ToArray() : new ICriteria[0]; }
        }


        /// <summary>
        /// Gets the list of packs which were dispensed by the output process.
        /// This property is set after the output process finished.
        /// </summary>
        [XmlIgnore]
        IDispensedPack[] IOutputProcessInfo.Packs
        {
            get { lock (_syncLock) { return _dispensedPackList.ToArray(); } }
        }

        /// <summary>
        /// Gets the list of boxes which were involved during pack dispensing.
        /// This property is set after the output process finished.
        /// </summary>
        [XmlIgnore]
        string[] IOutputProcessInfo.Boxes
        {
            get { lock (_syncLock) { return _boxList.ToArray(); } }
        }

        #endregion

        #region IOutputProcess Specific Events

        /// <summary>
        /// Event which is raised when the output process finished execution.
        /// </summary>
        /// <param name="sender">The output process instance which raised the event.</param>
        /// <param name="e">Not used. Always null.</param>
        public event EventHandler Finished
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for Finished as been registered for '{0}'", value.Method.Name);
                    _finished += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for Finished as been unregistered for '{0}'", value.Method.Name);
                    _finished -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised when the output process has box released.
        /// </summary>
        /// <param name="sender">The output process instance which raised the event.</param>
        /// <param name="e">Not used. Always null.</param>
        public event EventHandler BoxReleased
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for Box Released as been registered for '{0}'", value.Method.Name);
                    _boxReleased += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for Box Released as been unregistered for '{0}'", value.Method.Name);
                    _boxReleased -= value;
                }
            }
        }

        #endregion

        #region IOutputProcess Specific Methods

        /// <summary>
        /// Adds a new output criteria element to the output process.
        /// An output process can have multiple output criteria with any filter combination.
        /// </summary>
        /// <param name="articleId">
        /// The unique article identifier (e.g. PZN or EAN) filter criterion of the requested packs.
        /// </param>
        /// <param name="quantity">
        /// The amount of full packs to dispense.
        /// </param>
        /// <param name="batchNumber">
        /// The optional additional batch number filter criterion for the requested packs.
        /// </param>
        /// <param name="externalId">
        /// The optional additional external identifier filter criterion for the requested packs.
        /// </param>
        /// <param name="minimumExpiryDate">
        /// The optional additional filter criterion to request only packs that have at least the specified expiry date.
        /// </param>
        /// <param name="packId">
        /// The optional additional filter criterion which refers to the storage system internal pack identifier.
        /// Because of the fact that every pack in the storage system has a unique pack identifier,
        /// this filter criterion provides the possibility to dispense a specific pack.
        /// </param>
        /// <param name="subItemQuantity">
        /// The number of elements (e.g. pills or ampoules) to dispense. 
        /// If this parameter is set to a value greater than 0, the Quantity property is ignored and should be 0.
        /// </param>
        /// <param name="stockLocationId">
        /// The optional additional stock location filter criterion for the requested packs.
        /// </param>
        /// <param name="machineLocation">
        /// The optional additional machine location filter criterion for the requested packs.
        /// </param>
        /// <param name="singleBatchNumber">
        /// The optional additional flag that all of the requested packs have to belong to the same batch number.
        /// </param>
        void IOutputProcess.AddCriteria(string articleId, 
                                        uint quantity, 
                                        string batchNumber, 
                                        string externalId, 
                                        DateTime? minimumExpiryDate, 
                                        ulong packId, 
                                        uint subItemQuantity,
                                        string stockLocationId,
                                        string machineLocation,
                                        bool singleBatchNumber)
        {
            if ((quantity == 0) && (subItemQuantity == 0))
            {
                throw new ArgumentException("Invalid quantity parameter specified.");
            }

            if ((string.IsNullOrEmpty(articleId)) &&
                (string.IsNullOrEmpty(batchNumber)) &&
                (string.IsNullOrEmpty(externalId)) &&
                (minimumExpiryDate.HasValue == false) &&
                (packId == 0))
            {
                throw new ArgumentException("Invalid article or pack filter specified.");
            }

            lock (_syncLock)
            {
                if (_currentState != OutputProcessState.Created)
                {
                    throw new ApplicationException("Adding criteria to an active output process is not supported.");
                }
            }

            if (this.Criteria == null)
            {
                this.Criteria = new List<Criteria>();
            }

            this.Criteria.Add(new Criteria()
            {
                ArticleId = TextConverter.EscapeInvalidXmlChars(articleId),
                Quantity = quantity.ToString(),
                BatchNumber = TextConverter.EscapeInvalidXmlChars(batchNumber),
                ExternalId = TextConverter.EscapeInvalidXmlChars(externalId),
                MinimumExpiryDate = minimumExpiryDate.HasValue ? TypeConverter.ConvertDate(minimumExpiryDate.Value) : null,
                PackId = packId.ToString(),
                SubItemQuantity = subItemQuantity.ToString(),
                StockLocationId = TextConverter.EscapeInvalidXmlChars(stockLocationId),
                MachineLocation = TextConverter.EscapeInvalidXmlChars(machineLocation),
                SingleBatchNumber = singleBatchNumber.ToString()
            });
        }

        /// <summary>
        /// Starts the output process by sending the according request to the storage system.
        /// </summary>
        void IOutputProcess.Start()
        {
            lock (_syncLock)
            {
                if (_currentState != OutputProcessState.Created)
                {
                    return;
                }
            }

            var request = new OutputRequestEnvelope() { OutputRequest = this };
            var outputMessageInterceptor = new MessageInterceptor(this.Id, typeof(OutputMessage));
            _messageDispatcher.AddInterceptor(outputMessageInterceptor);

            var response = (OutputResponse)_messageDispatcher.SendAndWaitForResponse(request, 
                                                                                     this.Id, 
                                                                                     typeof(OutputResponse));

            if (response == null)
            {
                _messageDispatcher.RemoveInterceptor(outputMessageInterceptor);
                outputMessageInterceptor.Dispose();
                throw new ArgumentException("Waiting for the message 'OutputResponse' failed.");
            }

            if ((response.Details == null) || (Enum.TryParse<OutputProcessState>(response.Details.Status, out _currentState) == false))
            {
                _currentState = OutputProcessState.Unknown;
            }

            if (_currentState != OutputProcessState.Queued)
            {
                if (_finished != null)
                {
                    this.Trace("Raising event 'Finished'.");
                    _finished(this, new EventArgs());
                }
            }
            else
            {
                // wait for completion
                if (ThreadPool.QueueUserWorkItem(new WaitCallback(WaitForOutputMessage), 
                                                 outputMessageInterceptor) == false)
                {
                    throw new ApplicationException("Starting observation thread failed.");
                }
            }
        }

        /// <summary>
        /// Requests a cancellation of the output process.
        /// The cancellation is asynchronous and therefore the output process will need some time
        /// to finish with the state "Aborted".
        /// </summary>
        void IOutputProcess.Cancel()
        {
            lock (_syncLock)
            {
                if ((_currentState != OutputProcessState.Queued) &&
                    (_currentState != OutputProcessState.InProcess))
                {
                    return;
                }
            }

            var request = new TaskCancelRequestEnvelope()
            {
                TaskCancelRequest = new TaskCancelRequest()
                {
                    Id = MessageId.Next, 
                    Destination = this.Destination,
                    Source = this.Source
                }
            };

            request.TaskCancelRequest.Task = new Types.Task[] 
            { 
                new Types.Task() { Id = this.Id.ToString(), Type = TaskType.Output }
            };

            lock (_syncLock)
            {
                if ((_currentState == OutputProcessState.Queued) ||
                    (_currentState == OutputProcessState.InProcess))
                {
                    _currentState = OutputProcessState.Aborting;
                }
            }

            var response = (TaskCancelResponse)_messageDispatcher.SendAndWaitForResponse(request, 
                                                                                         request.TaskCancelRequest.Id, 
                                                                                         typeof(TaskCancelResponse));

            if (response == null)
            {
                throw new ArgumentException("Waiting for the message 'TaskCancelResponse' failed.");
            }

            if ((response.Task == null) || (response.Task[0] == null))
            {
                if (string.Compare(response.Task[0].Status, "CancelError", true) == 0)
                {
                    throw new ApplicationException("Cancellation of output process failed.");
                }
            }
        }

        #endregion

        #region Additional Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputRequest"/> class.
        /// </summary>
        public OutputRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputRequest"/> class.
        /// </summary>
        /// <param name="messageOjectStream"> The message object stream to use for sending the requests.</param>
        /// <param name="messageDispatcher">The message dispatcher to use for message interception.</param>
        public OutputRequest(XmlObjectStream messageOjectStream, MessageDispatcher messageDispatcher)
        {
            if (messageOjectStream == null)
            {
                throw new ArgumentException("Invalid messageOjectStream specified.");
            }

            if (messageDispatcher == null)
            {
                throw new ArgumentException("Invalid messageDispatcher specified.");
            }

            _messageOjectStream = messageOjectStream;
            _messageDispatcher = messageDispatcher;
            this.Details = new Details();
        }

        /// <summary>
        /// Asynchronous thread which waits for the completion of the specified output message interceptor.
        /// </summary>
        /// <param name="messageInterceptor">The message interceptor to wait for.</param>
        private void WaitForOutputMessage(object messageInterceptor)
        {
            try
            {
                using (var interceptor = messageInterceptor as MessageInterceptor)
                {
                    bool orderNotFinished = true;
                    while (orderNotFinished)
                    {
                        interceptor.Wait();

                        var outputMessage = (OutputMessage)interceptor.Message;

                        if (outputMessage != null)
                        {
                            lock (_syncLock)
                            {
                                if (outputMessage.Article != null)
                                {
                                    foreach (var article in outputMessage.Article)
                                    {
                                        if (article.Pack != null)
                                        {
                                            foreach (var pack in article.Pack)
                                            {
                                                pack.ArticleId = TextConverter.UnescapeInvalidXmlChars(article.Id);
                                            }

                                            _dispensedPackList.AddRange(article.Pack);
                                        }
                                    }
                                }

                                if (outputMessage.Box != null)
                                {
                                    foreach (var box in outputMessage.Box)
                                    {
                                        _boxList.Add(box.Number);
                                    }
                                }

                                if (Enum.TryParse<OutputProcessState>(outputMessage.Details.Status, out _currentState) == false)
                                {
                                    _currentState = OutputProcessState.Unknown;
                                }
                            }
                        }
                        else
                        {
                            this.Error("Waiting for the message 'OutputMessage' failed.");

                            lock (_syncLock)
                            {
                                _currentState = OutputProcessState.Unknown;
                            }
                        }

                        if (_currentState.Equals(OutputProcessState.BoxReleased))
                        {
                            if (_boxReleased != null)
                            {
                                lock (_syncLock)
                                {
                                    this.Trace("Raising event 'Box Released'.");
                                    _boxReleased(this, new EventArgs());
                                    _dispensedPackList.Clear();
                                }
                            }
                        }
                        else
                        {
                            orderNotFinished = false;
                            _messageDispatcher.RemoveInterceptor(interceptor);
                            if (_finished != null)
                            {
                                this.Trace("Raising event 'Finished'.");
                                _finished(this, new EventArgs());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Error("Receiving and processing 'OutputMessage' failed!", ex);
            }
        }

        #endregion
    }
}
