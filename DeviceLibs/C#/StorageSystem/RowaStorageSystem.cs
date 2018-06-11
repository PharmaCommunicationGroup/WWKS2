
using CareFusion.Lib.StorageSystem.Input;
using CareFusion.Lib.StorageSystem.Logging;
using CareFusion.Lib.StorageSystem.Net;
using CareFusion.Lib.StorageSystem.Output;
using CareFusion.Lib.StorageSystem.State;
using CareFusion.Lib.StorageSystem.Stock;
using CareFusion.Lib.StorageSystem.Wwks2;
using CareFusion.Lib.StorageSystem.Wwks2.Messages;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Configuration;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Hello;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Input;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.KeepAlive;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Output;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Status;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Stock;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Task;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace CareFusion.Lib.StorageSystem
{
    /// <summary>
    /// The storage system implementation for a CareFusion | Rowa system.
    /// </summary>
    public class RowaStorageSystem : AsyncEventProvider, IStorageSystem
    {
        #region Constants

        /// <summary>
        /// The subscriber identifier to use on WWKS 2.0 protocol level.
        /// </summary>
        private const int DefaultSubscriberID = 100;

        /// <summary>
        /// The connect timeout in milliseconds.
        /// </summary>
        private const int ConnectTimeout = 30000;

        /// <summary>
        /// The protocol handshake timeout in milliseconds.
        /// </summary>
        private const int HandshakeTimeout = 10000;

        /// <summary>
        /// The interval for checking the tcp connection in milliseconds.
        /// </summary>
        private const int ConnectionCheckInterval = 3000;

        /// <summary>
        /// The interval for requesting the state of the storage system in milliseconds.
        /// </summary>
        private const int StateCheckInterval = 15000;

        /// <summary>
        /// List of capabilities which are supported by Mosaic.
        /// </summary>
        private static Capability[] Capabilities = new Capability[] 
        { 
            new Capability() { Name="KeepAlive" },
            new Capability() { Name="Status" },
            new Capability() { Name="Input" },
            new Capability() { Name="InitiateInput" },
            new Capability() { Name="ArticleMaster" },
            new Capability() { Name="StockDelivery" },
            new Capability() { Name="StockInfo" },
            new Capability() { Name="Output" },
            new Capability() { Name="TaskInfo" },
            new Capability() { Name="TaskCancel" },
            new Capability() { Name="Configuration" },
            new Capability() { Name="StockLocationInfo" }
        };

        #endregion

        #region Members

        /// <summary>
        /// The tcp client of the active storage system connection. 
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// The object stream which is used to read and write messages.
        /// </summary>
        private XmlObjectStream _messageObjectStream;

        /// <summary>
        /// The subscriber identifier of the storage system which is retrieved during handshake.
        /// </summary>
        private int _destinationId;

        /// <summary>
        /// The tenant identifier to use during WWKS handshake.
        /// </summary>
        private string _tenantId = null;

        /// <summary>
        /// Holds the capabilities of the connected remote host.
        /// </summary>
        private Capability[] _destinationCapabilities;

        /// <summary>
        /// Event which is used to shutdown the background connection check thread.
        /// </summary>
        private ManualResetEvent _shutdownThreadEvent;

        /// <summary>
        /// The message dispatcher instance which reads and dispatches the messages of the active storage system connection.
        /// </summary>
        private MessageDispatcher _messageDispatcher = new MessageDispatcher();

        /// <summary>
        /// Flag whether automatic state observation is enabled.
        /// </summary>
        private bool _enableAutomaticStateObservation = true;

        /// <summary>
        /// The references to the internal event handles which are used for event registration.
        /// </summary>
        private StateChangedEventHandler _stateChanged;
        private ComponentStateChangedEventHandler _componentStateChanged;
        private PackDispensedEventHandler _packDispensed;
        private PackInputRequestEventHandler _packInputRequested;
        private PackStoredEventHandler _packStored;
        private PackInputFinished _packInputFinished;
        private StockUpdatedEventHandler _stockUpdated;
        private OutputProcessFinishedEventHandler _outputProcessFinished;

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private object _syncLock = new object();

        /// <summary>
        /// Flag whether the WWKS 2.0 related XML serializers were (pre)initialized.
        /// </summary>
        private static bool _serializerInitialized = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a flag whether the storage system is currently connected.
        /// </summary>
        public bool Connected
        {
            get 
            {
                lock (_syncLock)
                {
                    return (_client == null) ? false : _client.Connected;
                }
            }
        }

			/// <summary>
			/// Gets the subscriber id of the storage system.
			/// </summary>
			public int SubscriberID
			{
				get;

				private set;
			}


        /// <summary>
        /// Gets the current state of the storage system.
        /// </summary>
        public ComponentState State
        {
            get
            {
                if (this.Connected == false)
                {
                    return ComponentState.NotConnected;
                }

                var request = new StatusRequestEnvelope()
                {
                    StatusRequest = new StatusRequest()
                    {
                        Id = MessageId.Next,
                        Destination = _destinationId,
                        Source = SubscriberID,
                    }
                };

                var message = _messageDispatcher.SendAndWaitForResponse(request,
                                                                        request.StatusRequest.Id,
                                                                        typeof(StatusResponse));

                if (message == null)
                {
                    return ComponentState.NotConnected;
                }

                var statusResponse = (StatusResponse)message;
                return TypeConverter.ConvertEnum<ComponentState>(statusResponse.State, ComponentState.NotReady);
            }
        }

        /// <summary>
        /// Gets a list of all components of the storage system with their according states.
        /// </summary>
        public IComponent[] ComponentStates 
        {
            get 
            {
                if (this.Connected == false)
                {
                    return new IComponent[0];
                }

                var request = new StatusRequestEnvelope()
                {
                    StatusRequest = new StatusRequest()
                    {
                        Id = MessageId.Next,
                        Destination = _destinationId,
                        Source = SubscriberID,
                        IncludeDetails = true
                    }
                };

                var message = _messageDispatcher.SendAndWaitForResponse(request,
                                                                        request.StatusRequest.Id,
                                                                        typeof(StatusResponse));

                if (message == null)
                {
                    return new IComponent[0];
                }

                var statusResponse = (StatusResponse)message;

                return (statusResponse.Component != null) ? statusResponse.Component : new IComponent[0];
            }
        }

        /// <summary>
        /// Gets the arbitrary configuration content of the storage system.
        /// </summary>
        public string Configuration 
        { 
            get
            {
                if (this.Connected == false)
                {
                    return string.Empty;
                }

                bool isConfigurationSupported = false;

                if (_destinationCapabilities != null)
                {
                    foreach (var capability in _destinationCapabilities)
                    {
                        if (string.Compare(capability.Name, "Configuration", true) == 0)
                        {
                            isConfigurationSupported = true;
                        }
                    }
                }
                else
                {
                    isConfigurationSupported = true;
                }

                if (isConfigurationSupported == false)
                {
                    return string.Empty;
                }

                var request = new ConfigurationGetRequestEnvelope()
                {
                    ConfigurationGetRequest = new ConfigurationGetRequest()
                    {
                        Id = MessageId.Next,
                        Destination = _destinationId,
                        Source = SubscriberID,
                    }
                };

                var message = _messageDispatcher.SendAndWaitForResponse(request,
                                                                        request.ConfigurationGetRequest.Id,
                                                                        typeof(ConfigurationGetResponse),
                                                                        10000);

                if (message == null)
                {
                    return string.Empty;
                }

                var configurationResponse = (ConfigurationGetResponse)message;
                return configurationResponse.RawContent;
            }
        }

        /// <summary>
        /// Gets a list of all stock locations that are supported by the storage system.
        /// </summary>
        public IStockLocation[] StockLocations
        {
            get
            {
                if (this.Connected == false)
                {
                    return new IStockLocation[0];
                }

                var request = new StockLocationInfoRequestEnvelope()
                {
                    StockLocationInfoRequest = new StockLocationInfoRequest()
                    {
                        Id = MessageId.Next,
                        Destination = _destinationId,
                        Source = SubscriberID,
                    }
                };

                var message = _messageDispatcher.SendAndWaitForResponse(request,
                                                                        request.StockLocationInfoRequest.Id,
                                                                        typeof(StockLocationInfoResponse),
                                                                        10000);

                if (message == null)
                {
                    return new IStockLocation[0];
                }

                var response = (StockLocationInfoResponse)message;

                return (response.StockLocation != null) ? response.StockLocation : new IStockLocation[0];
            }
        }

        /// <summary>
        /// Enables or disables the automatic state observation.
        /// If it is enabled, the state of the storage system is requested regularly
        /// and in case of state changes the event "StateChanged" is raised.
        /// Automatic state observation is enabled by default.
        /// </summary>
        public bool EnableAutomaticStateObservation 
        {
            get { lock (_syncLock) { return _enableAutomaticStateObservation; } }
            set { lock (_syncLock) { _enableAutomaticStateObservation = value; } }
        }


        #endregion

        #region Events

        /// <summary>
        /// Event which is raised when the state of a storage system has changed.
        /// </summary>
        public event StateChangedEventHandler StateChanged
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for StateChanged as been registered for '{0}'", value.Method.Name);
                    _stateChanged += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for StateChanged as been unregistered for '{0}'", value.Method.Name);
                    _stateChanged -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised when the state of a storage system component has changed.
        /// </summary>
        public event ComponentStateChangedEventHandler ComponentStateChanged
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ComponentStateChanged as been registered for '{0}'", value.Method.Name);
                    _componentStateChanged += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ComponentStateChanged as been unregistered for '{0}'", value.Method.Name);
                    _componentStateChanged -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised when a pack was dispensed by an output
        /// process that was not initiated by this storage system connection (e.g. at the UI of the storage system).
        /// </summary>
        public event PackDispensedEventHandler PackDispensed
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for PackDispensed as been registered for '{0}'", value.Method.Name);
                    _packDispensed += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for PackDispensed as been unregistered for '{0}'", value.Method.Name);
                    _packDispensed -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised whenever a connected storage system requests permission for pack input.
        /// </summary>
        public event PackInputRequestEventHandler PackInputRequested
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for PackInputRequested as been registered for '{0}'", value.Method.Name);
                    _packInputRequested += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for PackInputRequested as been unregistered for '{0}'", value.Method.Name);
                    _packInputRequested -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised whenever a new pack was successfully stored in a storage system. 
        /// </summary>
        public event PackStoredEventHandler PackStored
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for PackStored as been registered for '{0}'", value.Method.Name);
                    _packStored += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for PackStored as been unregistered for '{0}'", value.Method.Name);
                    _packStored -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised whenever a connected storage system finished an input.
        /// </summary>
        public event PackInputFinished PackInputFinished
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for PackInputFinished as been registered for '{0}'", value.Method.Name);
                    _packInputFinished += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for PackInputFinished as been unregistered for '{0}'", value.Method.Name);
                    _packInputFinished -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised whenever the stock of the storage system has been updated.
        /// </summary>
        public event StockUpdatedEventHandler StockUpdated
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for StockUpdated as been registered for '{0}'", value.Method.Name);
                    _stockUpdated += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for StockUpdated as been unregistered for '{0}'", value.Method.Name);
                    _stockUpdated -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised whenever an output process finished processing.
        /// </summary>
        public event OutputProcessFinishedEventHandler OutputProcessFinished
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for OutputProcessFinished as been registered for '{0}'", value.Method.Name);
                    _outputProcessFinished += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for OutputProcessFinished as been unregistered for '{0}'", value.Method.Name);
                    _outputProcessFinished -= value;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RowaStorageSystem"/> class.
        /// </summary>
        public RowaStorageSystem( int subscriberID = RowaStorageSystem.DefaultSubscriberID )
        {
            LogManagerProxy.Initialize();
            LogVersion();
            InitializeXmlSerializer();
            _messageDispatcher.MessageArrived += OnMessageArrived;
            _messageDispatcher.MessageStreamDown += OnMessageStreamDown;

				this.SubscriberID = subscriberID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowaStorageSystem"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier to use during WWKS handshake.</param>
        public RowaStorageSystem(string tenantId, int subscriberID = RowaStorageSystem.DefaultSubscriberID )
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentException("Invalid tenantId specified.");
            }

            LogManagerProxy.Initialize();
            LogVersion();
            InitializeXmlSerializer();
            _messageDispatcher.MessageArrived += OnMessageArrived;
            _messageDispatcher.MessageStreamDown += OnMessageStreamDown;
            _tenantId = tenantId;

				this.SubscriberID = subscriberID;
        }

        /// <summary>
        /// Establishes a new connection to the storage system with the the specified host at the specified port.
        /// This method performs an implicit disconnect if there is already an active storage system connection.
        /// </summary>
        /// <param name="host">The name or ip address of the storage system.</param>
        /// <param name="port">The port number of the storage system. Default is 6050.</param>
        /// <param name="traceFileDirectory">Optional directory where to store the network trace files.</param>
        /// <param name="traceFileName">Optional base name for the network trace files.</param>
        public void Connect(string host, ushort port = 6050)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentException("Invalid host specified.");
            }

            if (port == 0)
            {
                throw new ArgumentException("Invalid port specified.");
            }

            Disconnect();

            lock (_syncLock)
            {
                this.Trace("Connecting to storage system '{0}' at port '{1}'.", host, port);

                _client = new TcpClient();
               
                var connectResult = _client.BeginConnect(host, (int)port, null, null);
               
                if (connectResult.AsyncWaitHandle.WaitOne(ConnectTimeout) == false)
                {
                    connectResult.AsyncWaitHandle.Close();
                    _client.Close();
                    _client = null;
                    this.Error("Connecting to storage system '{0}' at port '{1}' timed out.", host, port);
                    throw new TimeoutException(string.Format("Connecting to storage system '{0}' at port '{1}' timed out.", host, port));
                }

                _client.EndConnect(connectResult);
                _client.ReceiveTimeout = 0;
                _client.IncreaseReadBufferSize();
                _client.EnableSocketKeepAlive();

                // initialize message stream and register the supported XML message types
                _messageObjectStream = new XmlObjectStream(_client.GetStream(), "WWKS", "StorageSystem", string.Concat(host, ":", port));
                _messageObjectStream.AddSupportedType(typeof(KeepAliveRequestEnvelope), typeof(KeepAliveRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(KeepAliveResponseEnvelope), typeof(KeepAliveResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(HelloRequestEnvelope), typeof(HelloRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(HelloResponseEnvelope), typeof(HelloResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(InputMessageEnvelope), typeof(InputMessage).Name);
                _messageObjectStream.AddSupportedType(typeof(InputRequestEnvelope), typeof(InputRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(InputResponseEnvelope), typeof(InputResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(InitiateInputMessageEnvelope), typeof(InitiateInputMessage).Name);
                _messageObjectStream.AddSupportedType(typeof(InitiateInputRequestEnvelope), typeof(InitiateInputRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(InitiateInputResponseEnvelope), typeof(InitiateInputResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(StatusRequestEnvelope), typeof(StatusRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(StatusResponseEnvelope), typeof(StatusResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(StockInfoRequestEnvelope), typeof(StockInfoRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(TaskCancelRequestEnvelope), typeof(TaskCancelRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(TaskCancelResponseEnvelope), typeof(TaskCancelResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(TaskInfoRequestEnvelope), typeof(TaskInfoRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(TaskInfoResponseEnvelope), typeof(TaskInfoResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(ArticleMasterSetResponseEnvelope), typeof(ArticleMasterSetResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(StockDeliverySetResponseEnvelope), typeof(StockDeliverySetResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(OutputRequestEnvelope), typeof(OutputRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(OutputResponseEnvelope), typeof(OutputResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(OutputMessageEnvelope), typeof(OutputMessage).Name);
                _messageObjectStream.AddSupportedType(typeof(StockInfoResponseEnvelope), typeof(StockInfoResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(StockInfoMessageEnvelope), typeof(StockInfoMessage).Name);
                _messageObjectStream.AddSupportedType(typeof(ArticleMasterSetRequestEnvelope), typeof(ArticleMasterSetRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(StockDeliverySetRequestEnvelope), typeof(StockDeliverySetRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(ConfigurationGetRequestEnvelope), typeof(ConfigurationGetRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(ConfigurationGetResponseEnvelope), typeof(ConfigurationGetResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(StockLocationInfoRequestEnvelope), typeof(StockLocationInfoRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(StockLocationInfoResponseEnvelope), typeof(StockLocationInfoResponse).Name);
               
                if (ProcessHandshake() == false)
                {
                    Disconnect();
                    throw new ApplicationException("Processing WWKS protocol handshake failed.");
                }

                _messageDispatcher.Start(_messageObjectStream);
                _shutdownThreadEvent = new ManualResetEvent(false);

                if (ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectionCheck), _shutdownThreadEvent) == false)
                {
                    Disconnect();
                    throw new ApplicationException("Starting background connection check thread failed.");
                }
            }            
        }

        /// <summary>
        /// Performs a graceful shutdown of the storage system connection. 
        /// </summary>
        public void Disconnect()
        {
            lock (_syncLock)
            {
                if (_messageObjectStream == null)
                {
                    return;
                }

                this.Trace("Closing connection to storage system.");

                base.ResetEventQueue();

                if ((_shutdownThreadEvent != null) &&
                    (_shutdownThreadEvent.SafeWaitHandle.IsClosed == false))
                {
                    _shutdownThreadEvent.Set();                    
                }

                _shutdownThreadEvent = null;
                _messageDispatcher.Stop();
                _messageObjectStream.Dispose();
                _messageObjectStream = null;

                if (_client.Connected)
                {
                    try
                    {
                        _client.Client.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception ex)
                    {
                        this.Error("Shutdown of the connected TCP client failed!", ex);
                    }
                }

                _client.Close();
                _client = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// 

        protected override void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            base.Dispose(isDisposing);

            if (isDisposing)
            {
                Disconnect();

                _messageDispatcher.MessageStreamDown -= OnMessageStreamDown;
                _messageDispatcher.MessageArrived -= OnMessageArrived;
                _messageDispatcher.Dispose();
            }

            LogManagerProxy.Cleanup();
        }

        /// <summary>
        /// Gets the complete stock of the connected storage system. 
        /// </summary>
        /// <param name="includePackDetails">
        /// Flag whether the returned article list should also include detailed pack information.
        /// </param>
        /// <param name="includeArticleDetails">
        /// Flag whether the returned article list should also contain detailed article information 
        /// like name, dosage form, packaging unit, etc.
        /// </param>
        /// <returns>List of articles and packs which are currently stored in the storage system.</returns>
        public List<IArticle> GetStock(bool includePackDetails = true, 
                                       bool includeArticleDetails = false)
        {
            if (this.Connected == false)
            {
                throw new InvalidOperationException("The storage system is currently not connected.");
            }

            var request = new StockInfoRequestEnvelope()
            {
                StockInfoRequest = new StockInfoRequest() 
                {
                    Id = MessageId.Next,
                    Destination = _destinationId,
                    Source = SubscriberID,
                    IncludePacks = includePackDetails.ToString(),
                    IncludeArticleDetails = includeArticleDetails.ToString()
                }
            };

            var message = (StockInfoResponse)_messageDispatcher.SendAndWaitForResponse(request, 
                                                                                       request.StockInfoRequest.Id, 
                                                                                       typeof(StockInfoResponse));

            if (message == null)
            {
                throw new ApplicationException("Waiting for the message 'StockInfoResponse' failed.");
            }

            return (message.Article != null) ? new List<IArticle>(message.Article) : new List<IArticle>();
        }

        /// <summary>
        /// Gets the stock which matches the specified filter criteria from the connected storage system.
        /// </summary>
        /// <param name="includePackDetails">
        /// Flag whether the returned article list should also include detailed pack information.
        /// </param>
        /// <param name="includeArticleDetails">
        /// Flag whether the returned article list should also contain detailed article information 
        /// like name, dosage form, packaging unit, etc.
        /// </param>
        /// <param name="articleId">
        /// If this parameter is not null, the requested stock information is filtered by the specified article identifier.
        /// </param>
        /// <param name="batchNumber">
        /// If this parameter is not null, the requested stock information is filtered by the specified batch number.
        /// </param>
        /// <param name="externalId">
        /// If this parameter is not null, the requested stock information is filtered by the specified external pack identifier.
        /// </param>
        /// <returns>
        /// List of articles and packs which are currently stored in the storage system and matches the specified filter.
        /// </returns>
        public List<IArticle> GetStock(bool includePackDetails, 
                                       bool includeArticleDetails,
                                       string articleId, 
                                       string batchNumber = null, 
                                       string externalId = null,
                                       string stockLocationId = null,
                                       string machineLocation = null)
        {
            if (this.Connected == false)
            {
                throw new InvalidOperationException("The storage system is currently not connected.");
            }

            var request = new StockInfoRequestEnvelope()
            {
                StockInfoRequest = new StockInfoRequest()
                {
                    Id = MessageId.Next,
                    Destination = _destinationId,
                    Source = SubscriberID,
                    IncludePacks = includePackDetails.ToString(),
                    IncludeArticleDetails = includeArticleDetails.ToString()
                }
            };

            if ((string.IsNullOrEmpty(articleId) == false) || 
                (string.IsNullOrEmpty(batchNumber) == false) ||
                (string.IsNullOrEmpty(externalId) == false) ||
                (string.IsNullOrEmpty(stockLocationId) == false) ||
                (string.IsNullOrEmpty(machineLocation) == false))
            {
                var criteria = new Criteria() 
                { 
                    ArticleId = TextConverter.EscapeInvalidXmlChars(articleId), 
                    BatchNumber = TextConverter.EscapeInvalidXmlChars(batchNumber), 
                    ExternalId = TextConverter.EscapeInvalidXmlChars(externalId),
                    StockLocationId = TextConverter.EscapeInvalidXmlChars(stockLocationId),
                    MachineLocation = TextConverter.EscapeInvalidXmlChars(machineLocation)
                };
                request.StockInfoRequest.Criteria = new Criteria[] { criteria };
            }

            var message = (StockInfoResponse)_messageDispatcher.SendAndWaitForResponse(request,
                                                                                       request.StockInfoRequest.Id,
                                                                                       typeof(StockInfoResponse));

            if (message == null)
            {
                throw new ApplicationException("Waiting for the message 'StockInfoResponse' failed.");
            }

            return (message.Article != null) ? new List<IArticle>(message.Article) : new List<IArticle>();
        }

        /// <summary>
        /// Creates a new object instance of a master article.
        /// Master articles are used by the storage system to make self-sufficient decisions 
        /// about which articles are allowed to be stored in the storage system. If the storage
        /// system is configured to use its master article list, it will not send InputRequest
        /// for every scanned pack. Instead it will decide on its own whether to input a pack.
        /// </summary>
        /// <param name="articleId">The unique identifier of the article (e.g. PZN or EAN).</param>
        /// <param name="articleName">The name of the article.</param>
        /// <param name="dosageForm">The dosage form of the article.</param>
        /// <param name="packagingUnit">The packaging unit of the article.</param>
        /// <param name="requiresFridge">The flag whether packs of this article have to be stored in a refrigerator.</param>
        /// <param name="maxSubItemQuantity">
        /// The optional maximum number of elements (e.g. pills or ampoules) which are in one pack of this article.
        /// A value of 0 means "no maximum defined".
        /// </param>
        /// <param name="stockLocationId">Optional stock location to use for packs of this article.</param>
        /// <param name="machineLocation">Optional machine location to use for packs of this article.</param> 
        /// <returns>Newly created master article object instance.</returns>
        public IMasterArticle CreateMasterArticle(string articleId,
                                                  string articleName,
                                                  string dosageForm,
                                                  string packagingUnit,
                                                  bool requiresFridge = false,
                                                  uint maxSubItemQuantity = 0,
                                                  string stockLocationId = null,
                                                  string machineLocation = null)
        {
            if (string.IsNullOrEmpty(articleId))
            {
                throw new ArgumentException("Invalid articleId specified.");
            }

            if (string.IsNullOrEmpty(articleName))
            {
                throw new ArgumentException("Invalid articleName specified.");
            }

            return new Article()
            {
                Id = TextConverter.EscapeInvalidXmlChars(articleId),
                Name = TextConverter.EscapeInvalidXmlChars(articleName),
                DosageForm = string.IsNullOrEmpty(dosageForm) ? string.Empty : TextConverter.EscapeInvalidXmlChars(dosageForm),
                PackagingUnit = string.IsNullOrEmpty(packagingUnit) ? string.Empty : TextConverter.EscapeInvalidXmlChars(packagingUnit),
                RequiresFridge = requiresFridge.ToString(),
                MaxSubItemQuantity = maxSubItemQuantity.ToString(),
                StockLocationId = TextConverter.EscapeInvalidXmlChars(stockLocationId),
                MachineLocation = TextConverter.EscapeInvalidXmlChars(machineLocation)
            };
        }

        /// <summary>
        /// Updates the list of active master articles which are used by the storage system.
        /// </summary>
        /// <param name="masterArticleList">
        /// The new list of master articles to set. 
        /// If the list is empty, the master article list of the storage system is cleared.
        /// </param>
        public void UpdateMasterArticles(List<IMasterArticle> masterArticleList)
        {
            if (masterArticleList == null)
            {
                throw new ArgumentException("Invalid masterArticleList specified.");
            }

            if (this.Connected == false)
            {
                throw new InvalidOperationException("The storage system is currently not connected.");
            }

            var request = new ArticleMasterSetRequestEnvelope()
            {
                ArticleMasterSetRequest = new ArticleMasterSetRequest()
                {
                    Id = MessageId.Next,
                    Destination = _destinationId,
                    Source = SubscriberID,
                }
            };

            if (masterArticleList.Count > 0)
            {
                request.ArticleMasterSetRequest.Article = new Article[masterArticleList.Count];

                for (int i = 0; i < masterArticleList.Count; ++i)
                {
                    request.ArticleMasterSetRequest.Article[i] = (Article)masterArticleList[i];
                }
            }

            var message = (ArticleMasterSetResponse)_messageDispatcher.SendAndWaitForResponse(request,
                                                                                              request.ArticleMasterSetRequest.Id,
                                                                                              typeof(ArticleMasterSetResponse));

            if (message == null)
            {
                throw new ApplicationException("Waiting for the message 'ArticleMasterSetResponse' failed.");
            }

            if ((message.SetResult != null) &&
                (message.SetResult.Value != SetResultValue.Accepted))
            {
                var errorMessage = string.Format("The specified master articles were rejected by the storage " +
                                                 "system with the reason '{0}'.", TextConverter.UnescapeInvalidXmlChars(message.SetResult.Text));

                throw new ApplicationException(errorMessage);
            }
        }

        /// <summary>
        /// Creates a new object instance of a stock delivery.
        /// Pre-defined stock deliveries are used by the storage system to make self-sufficient decisions 
        /// about which stock deliveries are allowed to be started at the storage system and which
        /// articles are allowed to be stored in combination with a specific stock delivery. 
        /// If the storage system is configured to use pre-defined stock deliveries, it will not 
        /// send InputRequest for every scanned pack that belongs to a stock delivery. 
        /// Instead it will decide on its own whether to input a pack.
        /// </summary>
        /// <param name="deliveryNumber">The delivery number of the stock delivery.</param>
        /// <returns>Newly created stock delivery object instance.</returns>
        public IStockDelivery CreateStockDelivery(string deliveryNumber)
        {
            if (string.IsNullOrEmpty(deliveryNumber))
            {
                throw new ArgumentException("Invalid deliveryNumber specified.");
            }

            return new StockDelivery() 
            { 
                DeliveryNumber = deliveryNumber,
                Article = new List<Article>()
            };
        }

        /// <summary>
        /// Adds the specified list of stock deliveries to the pre-defined stock delivery list of the storage system.
        /// </summary>
        /// <param name="stockDeliveryList">List of stock deliveries to add.</param>
        public void AddStockDeliveries(List<IStockDelivery> stockDeliveryList)
        {
            if (stockDeliveryList == null)
            {
                throw new ArgumentException("Invalid stockDeliveryList specified.");
            }

            if (this.Connected == false)
            {
                throw new InvalidOperationException("The storage system is currently not connected.");
            }

            var request = new StockDeliverySetRequestEnvelope()
            {
                StockDeliverySetRequest = new StockDeliverySetRequest()
                {
                    Id = MessageId.Next,
                    Destination = _destinationId,
                    Source = SubscriberID,
                }
            };

            if (stockDeliveryList.Count > 0)
            {
                request.StockDeliverySetRequest.StockDelivery = new StockDelivery[stockDeliveryList.Count];

                for (int i = 0; i < stockDeliveryList.Count; ++i)
                {
                    request.StockDeliverySetRequest.StockDelivery[i] = (StockDelivery)stockDeliveryList[i];
                }
            }

            var message = (StockDeliverySetResponse)_messageDispatcher.SendAndWaitForResponse(request,
                                                                                              request.StockDeliverySetRequest.Id,
                                                                                              typeof(StockDeliverySetResponse));

            if (message == null)
            {
                throw new ApplicationException("Waiting for the message 'StockDeliverySetResponse' failed.");
            }

            if ((message.SetResult != null) &&
                (message.SetResult.Value != SetResultValue.Accepted))
            {
                var errorMessage = string.Format("The specified stock deliveries were rejected by the storage " +
                                                 "system with the reason '{0}'.", TextConverter.UnescapeInvalidXmlChars(message.SetResult.Text));

                throw new ApplicationException(errorMessage);
            }
        }

        /// <summary>
        /// Gets detailed information about the specified stock delivery.
        /// </summary>
        /// <param name="deliveryNumber">The delivery number of the stock delivery.</param>
        /// <returns>According stock delivery information.</returns>
        public IStockDeliveryInfo GetStockDeliveryInfo(string deliveryNumber)
        {
            if (this.Connected == false)
            {
                throw new InvalidOperationException("The storage system is currently not connected.");
            }

            var request = new TaskInfoRequestEnvelope()
            {
                TaskInfoRequest = new TaskInfoRequest()
                {
                    Id = MessageId.Next,
                    Destination = _destinationId,
                    Source = SubscriberID,
                    IncludeTaskDetails = true.ToString()
                }
            };

            request.TaskInfoRequest.Task = new Task[1];
            request.TaskInfoRequest.Task[0] = new Task() { Id = deliveryNumber, Type = TaskType.StockDelivery };

            var message = _messageDispatcher.SendAndWaitForResponse(request,
                                                                    request.TaskInfoRequest.Id,
                                                                    typeof(TaskInfoResponse));

            var taskInfoResponse = (TaskInfoResponse)message;

            if ((taskInfoResponse == null) ||
                (taskInfoResponse.Task == null) ||
                (taskInfoResponse.Task.Length == 0) ||
                (taskInfoResponse.Task[0] == null))
            {
                var errorMessage = string.Format("Requesting information for stock delivery {0} failed.", deliveryNumber);
                throw new ApplicationException(errorMessage);
            }

            return taskInfoResponse.Task[0];
        }

        /// <summary>
        /// Creates a new output process which can be used to dispense packs from the connected storage system.
        /// </summary>
        /// <param name="orderNumber">The unique order number which is used to identify an output process.</param>
        /// <param name="outputDestination">The storage system output destination for the dispensed packs.
        /// This number typically refers to a dispense point which has been configured at the storage system
        /// (e.g. 1 = maintenance output).</param>
        /// <param name="outputPoint">A more detailed definition of the storage system output destination (e.g. belt number) for the dispensed packs.</param>
        /// <param name="priority">The priority of this output process.</param>
        /// <param name="boxNumber">The optional box number which belongs to this output process.
        /// This parameter is required for box system scenarios when the IT system
        /// wants to dispense packs to a specific box.</param>
        /// <returns>
        /// Newly created output process to use for dispensing packs.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The storage system is currently not connected.</exception>
        public IOutputProcess CreateOutputProcess(int orderNumber, 
                                                  int outputDestination, 
                                                  int outputPoint = 0,
                                                  OutputProcessPriority priority = OutputProcessPriority.Normal, 
                                                  string boxNumber = null)
        {
            return CreateOutputProcess(orderNumber.ToString(),
                                       outputDestination,
                                       outputPoint,
                                       priority,
                                       boxNumber);
        }

        /// <summary>
        /// Creates a new output process which can be used to dispense packs from the connected storage system.
        /// </summary>
        /// <param name="orderNumber">The unique order number which is used to identify an output process.</param>
        /// <param name="outputDestination">The storage system output destination for the dispensed packs.
        /// This number typically refers to a dispense point which has been configured at the storage system
        /// (e.g. 1 = maintenance output).</param>
        /// <param name="outputPoint">A more detailed definition of the storage system output destination (e.g. belt number) for the dispensed packs.</param>
        /// <param name="priority">The priority of this output process.</param>
        /// <param name="boxNumber">The optional box number which belongs to this output process.
        /// This parameter is required for box system scenarios when the IT system
        /// wants to dispense packs to a specific box.</param>
        /// <returns>
        /// Newly created output process to use for dispensing packs.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The storage system is currently not connected.</exception>
        public IOutputProcess CreateOutputProcess(string orderNumber,
                                                  int outputDestination,
                                                  int outputPoint = 0,
                                                  OutputProcessPriority priority = OutputProcessPriority.Normal,
                                                  string boxNumber = null)
        {
            lock (_syncLock)
            {
                if (this.Connected == false)
                {
                    throw new InvalidOperationException("The storage system is currently not connected.");
                }

                var result = new OutputRequest(_messageObjectStream, _messageDispatcher)
                {
                    Id = orderNumber,
                    Source = SubscriberID,
                    Destination = _destinationId
                };

                result.Details = new Details()
                {
                    OutputDestination = outputDestination.ToString(),
                    OutputPoint = outputPoint.ToString(),
                    Priority = priority.ToString()
                };

                result.BoxNumber = boxNumber;
                result.Finished += OnOutputProcess_Finished;
                return result;
            }
        }        

        /// <summary>
        /// Gets detailed information about the specified output process.
        /// </summary>
        /// <param name="orderNumber">The unique order number which identifies the output process.</param>
        /// <returns>
        /// According output process information.
        /// </returns>
        public IOutputProcessInfo GetOutputProcessInfo(int orderNumber)
        {
            return GetOutputProcessInfo(orderNumber.ToString());
        }

        /// <summary>
        /// Gets detailed information about the specified output process.
        /// </summary>
        /// <param name="orderNumber">The unique order number which identifies the output process.</param>
        /// <returns>
        /// According output process information.
        /// </returns>
        public IOutputProcessInfo GetOutputProcessInfo(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                throw new ArgumentException("Invalid orderNumber specified.");
            }                

            if (this.Connected == false)
            {
                throw new InvalidOperationException("The storage system is currently not connected.");
            }

            var request = new TaskInfoRequestEnvelope()
            {
                TaskInfoRequest = new TaskInfoRequest()
                {
                    Id = MessageId.Next,
                    Destination = _destinationId,
                    Source = SubscriberID,
                    IncludeTaskDetails = true.ToString()
                }
            };

            request.TaskInfoRequest.Task = new Task[1];
            request.TaskInfoRequest.Task[0] = new Task() { Id = orderNumber, Type = TaskType.Output };

            var message = _messageDispatcher.SendAndWaitForResponse(request,
                                                                    request.TaskInfoRequest.Id,
                                                                    typeof(TaskInfoResponse));

            var taskInfoResponse = (TaskInfoResponse)message;

            if ((taskInfoResponse == null) ||
                (taskInfoResponse.Task == null) ||
                (taskInfoResponse.Task.Length == 0) ||
                (taskInfoResponse.Task[0] == null))
            {
                var errorMessage = string.Format("Requesting information for output process {0} failed.", orderNumber);
                throw new ApplicationException(errorMessage);
            }

            return taskInfoResponse.Task[0];
        }

        /// <summary>
        /// Creates a new input initiation request which can be used to trigger an input request of the connected storage system.
        /// </summary>
        /// <param name="id">The unique identifier of the initiated input process.</param>
        /// <param name="inputSource">The storage system input source for the pack input.</param>
        /// <param name="destination">The destination identifier of the system that should start the input process.</param>
        /// <param name="deliveryNumber">Delivery number to use when the initiated input is part of a new delivery.
        /// A value of null means that the input is part of a stock return.</param>
        /// <param name="setPickingIndicator">Flag whether the initiated input should request the activation of the picking indicator.</param>
        /// <returns>
        /// Newly created input initiation request if initiation of inputs is supported;null otherwise.
        /// </returns>
        public IInitiateInputRequest CreateInitiateInputRequest(int id,
                                                                int inputSource,
                                                                int inputPoint = 0,
                                                                int destination = 0,
                                                                string deliveryNumber = null,
                                                                bool setPickingIndicator = false)
        {
            lock (_syncLock)
            {
                if (this.Connected == false)
                {
                    throw new InvalidOperationException("The storage system is currently not connected.");
                }

                bool isInitiateInputSupported = false;

                if (_destinationCapabilities != null)
                {
                    foreach (var capability in _destinationCapabilities)
                    {
                        if (string.Compare(capability.Name, "InitiateInput", true) == 0)
                        {
                            isInitiateInputSupported = true;
                        }
                    }
                }
                else
                {
                    isInitiateInputSupported = true;
                }

                if (isInitiateInputSupported == false)
                {
                    return null;
                }

                var result = new InitiateInputRequest(_messageObjectStream, _messageDispatcher, deliveryNumber)
                {
                    Id = id.ToString(),
                    Source = SubscriberID,
                    Destination = (destination == 0) ? _destinationId : destination
                };

                result.SetPickingIndicator = setPickingIndicator.ToString();
                result.Details = new Details() 
                { 
                    InputSource = inputSource.ToString(), 
                    InputPoint = inputPoint.ToString() 
                };

                return result;
            }
        }

        /// <summary>
        /// Processes the WWKS 2.0 protocol handshake to initialize the connection to the storage system.
        /// </summary>
        private bool ProcessHandshake()
        {
            HelloRequestEnvelope request = new HelloRequestEnvelope()
            {
                HelloRequest = new HelloRequest()
                {
                    Id = MessageId.Next,                    
                    Subscriber = new Subscriber()
                    {
                        Id = SubscriberID,
                        Type = SubscriberType.IMS,
                        Manufacturer = "CareFusion Germany 326 GmbH",
                        ProductInfo = "StorageSystem Library",
                        VersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion,
                        Capability = Capabilities,
                        TenantId = _tenantId
                    }
                }
            };

            if (_messageObjectStream.Write(request) == false)
            {
                this.Error("Sending the initial 'HelloRequest' message failed!");
                return false;
            }

            object response = _messageObjectStream.Read(HandshakeTimeout);

            if (response == null)
            {
                this.Error("Waiting for the message 'HelloResponse' failed.");
                return false;
            }

            if (response.GetType() != typeof(HelloResponseEnvelope))
            {
                this.Error("Received unexpected message of type '{0}'.", response.GetType().Name);
                return false;
            }

            HelloResponseEnvelope resp = (HelloResponseEnvelope)response;

            if (resp.HelloResponse.Subscriber == null)
            {
                this.Error("Received a 'HelloResponse' message with invalid subscriber information.");
                return false;
            }

            if (resp.HelloResponse.Id != request.HelloRequest.Id)
            {
                this.Error("Received a 'HelloResponse' message with invalid message identifier.");
                return false;
            }

            _destinationCapabilities = resp.HelloResponse.Subscriber.Capability;

            this.Info("WWKS 2.0 protocol handshake successfully finished -> ID='{0}' Type='{1}' " +
                      "Manufacturer='{2}' ProductInfo='{3}' VersionInfo='{4}'.",
                      resp.HelloResponse.Subscriber.Id, resp.HelloResponse.Subscriber.Type.ToString(),
                      string.IsNullOrEmpty(resp.HelloResponse.Subscriber.Manufacturer) ? string.Empty : resp.HelloResponse.Subscriber.Manufacturer,
                      string.IsNullOrEmpty(resp.HelloResponse.Subscriber.ProductInfo) ? string.Empty : resp.HelloResponse.Subscriber.ProductInfo,
                      string.IsNullOrEmpty(resp.HelloResponse.Subscriber.VersionInfo) ? string.Empty : resp.HelloResponse.Subscriber.VersionInfo);

            _destinationId = resp.HelloResponse.Subscriber.Id;
            return true;
        }

        /// <summary>
        /// Event which is raised when the message reader detected a problem while reading messages from the stream.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Always null and not used here.</param>
        private void OnMessageStreamDown(object sender, EventArgs e)
        {
            this.Trace("Message stream connection went down.");
            Disconnect();
        }

        /// <summary>
        /// Event which is raised when a new message arrived that is not handled by any pending message interceptors.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Instance of MessageArrivedArgs with event details.</param>
        private void OnMessageArrived(object sender, EventArgs e)
        {
            if (sender != _messageDispatcher)
            {
                return;
            }

            var eventArgs = e as MessageDispatcher.MessageArrivedArgs;

            if (eventArgs == null)
            {
                return;
            }

            try
            {
                switch (eventArgs.Message.GetType().Name)
                {
                    case "InputRequest": ProcessInputRequest((InputRequest)eventArgs.Message); break;
                    case "InputMessage": ProcessInputMessage((InputMessage)eventArgs.Message); break;
                    case "OutputMessage": ProcessOutputMessage((OutputMessage)eventArgs.Message); break;
                    case "StockInfoMessage": ProcessStockInfoMessage((StockInfoMessage)eventArgs.Message); break;
                }
            }
            catch (Exception ex)
            {
                this.Error("Processing incomming message '{0}' failed!", ex, eventArgs.Message.GetType().Name);         
            }
        }

        /// <summary>
        /// Processes an incomming pack input request message.
        /// </summary>
        /// <param name="request">The request message to process.</param>
        private void ProcessInputRequest(InputRequest request)
        {
            lock (_syncLock)
            {
                if (_messageObjectStream == null)
                {
                    return;
                }

                request.MessageObjectStream = _messageObjectStream;
            }
                
            base.Raise("PackInputRequested", _packInputRequested, this, request);
        }

        /// <summary>
        /// Processes a pack input notification message.
        /// </summary>
        /// <param name="request">The pack input notification message to process.</param>
        private void ProcessInputMessage(InputMessage message)
        {
            if ((message.Article == null) || (message.Article.Count == 0))
            {
                return;
            }

            var articles = new List<IArticle>();

            foreach (var article in message.Article)
            {
                if ((article.Pack == null) || (article.Pack.Count == 0))
                {
                    continue;
                }

                foreach (var pack in article.Pack.ToArray())
                {
                    if ((pack.Handling == null) || 
                        (string.Compare(pack.Handling.Input, "Completed", true) != 0))
                    {
                        article.Pack.Remove(pack);
                    }
                }

                if (article.Pack.Count > 0)
                {
                    articles.Add(article);
                }
            }

            if (articles.Count > 0)
            {
                base.Raise("PackStored", _packStored, this, articles.ToArray());             
            }       

            if (_packInputFinished != null)
            {
                base.Raise("PackInputFinished",
                           _packInputFinished, 
                           this,
                           message.Source,
                           int.Parse(message.Id),
                           (articles.Count > 0) ? InputResult.Completed : InputResult.Aborted,
                           articles.ToArray());
            }     
        }

        /// <summary>
        /// Processes a pack output notification message.
        /// </summary>
        /// <param name="message">The pack output notification message to process.</param>
        private void ProcessOutputMessage(OutputMessage message)
        {
            if (_outputProcessFinished != null)
            {
                if (message.Id == "1")
                {
                    if ((message.Article != null) && (message.Article.Length > 0))
                    {
                        base.Raise("PackDispensed", _packDispensed, this, message.Article);
                    }
                }
                else
                {
                    base.Raise("OutputProcessFinished", _outputProcessFinished, this, message.Id, message);
                }
            }
            else
            {
                if ((message.Article != null) && (message.Article.Length > 0))
                {
                    base.Raise("PackDispensed", _packDispensed, this, message.Article);
                }
            }         
        }

        /// <summary>
        /// Called when an output process has finished execution.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnOutputProcess_Finished(object sender, EventArgs e)
        {
            var outputRequest = (OutputRequest)sender;
            if (_outputProcessFinished != null)
            {
                base.Raise("OutputProcessFinished", _outputProcessFinished, this, outputRequest.Id, outputRequest);
            }
        }

        /// <summary>
        /// Processes the event of stock updates.
        /// </summary>
        /// <param name="message">The stock update message to process.</param>
        private void ProcessStockInfoMessage(StockInfoMessage message)
        {
            if ((message.Article != null) && (message.Article.Length > 0))
            {
                base.Raise("StockUpdated", _stockUpdated, this, message.Article);
            }
        }

        /// <summary>
        /// Processes regulary tcp connection checks and polls for the state of the storage system.
        /// </summary>
        /// <param name="parameter">Instance of the RowaStorageSystem object to observe.</param>
        private void ConnectionCheck(object parameter)
        {
            this.Trace("Background connection check thread started.");
            var shutdownEvent = parameter as ManualResetEvent;
            var currentState = ComponentState.NotConnected;
            var currentComponents = new IComponent[0];
            var lastState = currentState;

            try
            {                                
                var lastComponents = currentComponents;
                var stateTime = DateTime.UtcNow;
                
                if (this.EnableAutomaticStateObservation)
                {
                    if (_stateChanged != null)
                    {
                        currentState = this.State;
                        lastState = currentState;
                        base.Raise("StateChanged", _stateChanged, this, currentState);
                    }

                    if (_componentStateChanged != null)
                    {
                        currentComponents = this.ComponentStates;
                        lastComponents = currentComponents;

                        foreach (var component in currentComponents)
                            base.Raise("ComponentStateChanged", _componentStateChanged, this, component);
                    }
                }

                while (this.Connected)
                {
                    if (this.EnableAutomaticStateObservation)
                    {
                        if ((DateTime.UtcNow - stateTime).TotalMilliseconds >= StateCheckInterval)
                        {
                            if (_stateChanged != null)
                                currentState = this.State;

                            if (_componentStateChanged != null)
                                currentComponents = this.ComponentStates;

                            stateTime = DateTime.UtcNow;

                            if ((_stateChanged != null) &&
                                (lastState != currentState))
                            {
                                lastState = currentState;
                                base.Raise("StateChanged", _stateChanged, this, currentState);
                            }

                            if (_componentStateChanged != null)
                            {
                                for (int i = 0; i < lastComponents.Length; ++i)
                                {
                                    if ((currentComponents.Length > i) &&
                                        ((currentComponents[i].Description != lastComponents[i].Description) ||
                                         (currentComponents[i].State != lastComponents[i].State)))
                                    {
                                        base.Raise("ComponentStateChanged", _componentStateChanged, this, currentComponents[i]);
                                    }
                                }

                                lastComponents = currentComponents;
                            }
                           
                        }
                    }

                    if (shutdownEvent.WaitOne(ConnectionCheckInterval))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Error("Processing connection and status checks failed!", ex);
            }
            finally
            {
                shutdownEvent.Dispose();

                try
                {
                    if ((_stateChanged != null) &&
                        (this.EnableAutomaticStateObservation) &&
                        (lastState != ComponentState.NotConnected))
                    {
                        base.Raise("StateChanged", _stateChanged, this, ComponentState.NotConnected);
                    }  
                }
                catch (Exception ex) 
                {
                    this.Error("Raising final StateChanged event failed!", ex);
                }                       
            }

            this.Trace("Background connection check thread stopped.");
        }

        /// <summary>
        /// Logs the version number of the Dll in a secure manner.
        /// </summary>
        private void LogVersion()
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var today = DateTime.Now;
                var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                var hashInput = string.Format($"{version}-{today.Day}.{today.Month}.{today.Year}");
                var hashBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hashInput));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "");

                var log = string.Format($"StorageSystem Version: {version} Hash: {hashString}");
                this.Info(log);
            }
        }

        /// <summary>
        /// (Pre)Initializes the XML serializer for the required WWKS 2.0 messages.
        /// This one-time initialization will speed up the execution time later on
        /// when creating new RowaStorageSystem instances.
        /// 
        /// The XmlSerializer COMPILES! temporary intermediate assemblies for every
        /// type you want to serialize/deserialize with it. This happens in the background
        /// when you instantiate the XmlSerializer for a specific type for the very first time.
        /// </summary>
        /// <returns><c>true</c> if initialization was successful; <c>false</c> otherwise.</returns>
        private bool InitializeXmlSerializer()
        {
            if (_serializerInitialized)
            {
                return true;
            }

            this.Trace("Initializing WWKS 2.0 XmlSerializer ...");

            try
            {
                var v1 = new XmlSerializer(typeof(HelloRequestEnvelope));
                var v2 = new XmlSerializer(typeof(HelloResponseEnvelope));
                var v3 = new XmlSerializer(typeof(InputMessageEnvelope));
                var v4 = new XmlSerializer(typeof(InputRequestEnvelope));
                var v5 = new XmlSerializer(typeof(InputResponseEnvelope));
                var v6 = new XmlSerializer(typeof(OutputMessageEnvelope));
                var v7 = new XmlSerializer(typeof(OutputRequestEnvelope));
                var v8 = new XmlSerializer(typeof(OutputResponseEnvelope));
                var v9 = new XmlSerializer(typeof(StatusRequestEnvelope));
                var v10 = new XmlSerializer(typeof(StatusResponseEnvelope));
                var v11 = new XmlSerializer(typeof(StockInfoRequestEnvelope));
                var v12 = new XmlSerializer(typeof(StockInfoResponseEnvelope));
                var v13 = new XmlSerializer(typeof(StockInfoMessageEnvelope));
                var v14 = new XmlSerializer(typeof(ArticleMasterSetRequestEnvelope));
                var v15 = new XmlSerializer(typeof(ArticleMasterSetResponseEnvelope));
                var v16 = new XmlSerializer(typeof(StockDeliverySetRequestEnvelope));
                var v17 = new XmlSerializer(typeof(StockDeliverySetResponseEnvelope));
                var v18 = new XmlSerializer(typeof(TaskCancelRequestEnvelope));
                var v19 = new XmlSerializer(typeof(TaskCancelResponseEnvelope));
                var v20 = new XmlSerializer(typeof(TaskInfoRequestEnvelope));
                var v21 = new XmlSerializer(typeof(TaskInfoResponseEnvelope));
                var v22 = new XmlSerializer(typeof(KeepAliveRequestEnvelope));
                var v23 = new XmlSerializer(typeof(KeepAliveResponseEnvelope));
                var v24 = new XmlSerializer(typeof(InitiateInputRequestEnvelope));
                var v25 = new XmlSerializer(typeof(InitiateInputResponseEnvelope));
                var v26 = new XmlSerializer(typeof(InitiateInputRequestEnvelope));
                var v27 = new XmlSerializer(typeof(ConfigurationGetRequestEnvelope));
                var v28 = new XmlSerializer(typeof(ConfigurationGetResponseEnvelope));
                var v29 = new XmlSerializer(typeof(StockLocationInfoRequestEnvelope));
                var v30 = new XmlSerializer(typeof(StockLocationInfoResponseEnvelope));
                
                this.Trace("Initialization of WWKS 2.0 XmlSerializer finished.");
                _serializerInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                this.Error("Initializing WWKS 2.0 XmlSerializer failed!", ex);
            }

            return false;
        }

        #endregion
                
    }
}
