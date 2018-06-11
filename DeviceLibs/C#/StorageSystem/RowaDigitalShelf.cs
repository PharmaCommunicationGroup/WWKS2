using CareFusion.Lib.StorageSystem.Logging;
using CareFusion.Lib.StorageSystem.Net;
using CareFusion.Lib.StorageSystem.Sales;
using CareFusion.Lib.StorageSystem.Wwks2;
using CareFusion.Lib.StorageSystem.Wwks2.Messages;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.ArticleInformation;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Hello;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Sales;
using CareFusion.Lib.StorageSystem.Wwks2.Messages.Stock;
using CareFusion.Lib.StorageSystem.Wwks2.Types;
using CareFusion.Lib.StorageSystem.Wwks2.Types.DigitalShelf;
using CareFusion.Lib.StorageSystem.Xml;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using Rowa.Lib.Log;

namespace CareFusion.Lib.StorageSystem
{
    /// <summary>
    /// The digital shelf implementation for a Rowa digital shelf system (e.g. Rowa VMotion).
    /// </summary>
    public class RowaDigitalShelf : AsyncEventProvider, IDigitalShelf
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

        #endregion

        #region Members

        /// <summary>
        /// The message dispatcher instance which reads and dispatches the messages of the active storage system connection.
        /// </summary>
        private MessageDispatcher _messageDispatcher = new MessageDispatcher();

        /// <summary>
        /// The tcp client of the active storage system connection. 
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// The object stream which is used to read and write messages.
        /// </summary>
        private XmlObjectStream _messageObjectStream;

        /// <summary>
        /// Event which is used to shutdown the background connection check thread.
        /// </summary>
        private ManualResetEvent _shutdownThreadEvent;

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private object _syncLock = new object();

        /// <summary>
        /// Flag whether the WWKS 2.0 related XML serializers were (pre)initialized.
        /// </summary>
        private static bool _serializerInitialized = false;

        /// <summary>
        /// The subscriber identifier of the digital shelf which is retrieved during handshake.
        /// </summary>
        private int _destinationId;

        /// <summary>
        /// Holds the capabilities of the connected remote host.
        /// </summary>
        private Capability[] _destinationCapabilities;

        private ArticleSelectedEventHandler _articleSelected;
        private ArticleInfoRequestEventHandler _articleInfoRequested;
        private ArticlePriceRequestEventHandler _articlePriceRequested;
        private ShoppingCartRequestEventHandler _shoppingCartRequested;
        private ShoppingCartUpdateRequestEventHandler _shoppingCartUpdateRequested;
        private StockInfoRequestEventHandler _stockInfoRequested;

        /// <summary>
        /// List of capabilities which are supported by Mosaic.
        /// </summary>
        private Capability[] Capabilities = new Capability[]
        {
            new Capability() { Name="ArticleInfo" },
            new Capability() { Name="ArticlePrice" },
            new Capability() { Name="ShoppingCart" },
            new Capability() { Name="ArticleSelection" },
            new Capability() { Name="StockInfo" }
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets a flag whether the digital shelf is currently connected.
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
			/// Gets the subscriber id of the digital shelf.
			/// </summary>
		  public int SubscriberID
		  {
				get;

				private set;
		  }

        #endregion

        #region Events

        /// <summary>
        /// Event which is raised when an article has been selected on the screen of the digital shelf.
        /// </summary>
        public event ArticleSelectedEventHandler ArticleSelected
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ArticleSelected as been registered for '{0}'", value.Method.Name);
                    _articleSelected += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ArticleSelected as been unregistered for '{0}'", value.Method.Name);
                    _articleSelected -= value;

                }
            }
        }

        /// <summary>
        /// Event which is raised when a digital shelf requests detailed information for one or more articles.
        /// </summary>
        public event ArticleInfoRequestEventHandler ArticleInfoRequested
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ArticleInfoRequested as been registered for '{0}'", value.Method.Name);
                    _articleInfoRequested += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ArticleInfoRequested as been unregistered for '{0}'", value.Method.Name);
                    _articleInfoRequested -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised when a digital shelf requests price information for one or more articles.
        /// </summary>
        public event ArticlePriceRequestEventHandler ArticlePriceRequested
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ArticlePriceRequested as been registered for '{0}'", value.Method.Name);
                    _articlePriceRequested += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ArticlePriceRequested as been unregistered for '{0}'", value.Method.Name);
                    _articlePriceRequested -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised when a digital shelf requests a shopping cart.
        /// This can either be a new, empty shopping cart or an existing one (depending on the critiera specified).
        /// </summary>
        public event ShoppingCartRequestEventHandler ShoppingCartRequested
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ShoppingCartRequested as been registered for '{0}'", value.Method.Name);
                    _shoppingCartRequested += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ShoppingCartRequested as been unregistered for '{0}'", value.Method.Name);
                    _shoppingCartRequested -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised when a shopping cart has been manipulated on a digital shelf.
        /// </summary>
        public event ShoppingCartUpdateRequestEventHandler ShoppingCartUpdateRequested
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ShoppingCartUpdateRequested as been registered for '{0}'", value.Method.Name);
                    _shoppingCartUpdateRequested += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for ShoppingCartUpdateRequested as been unregistered for '{0}'", value.Method.Name);
                    _shoppingCartUpdateRequested -= value;
                }
            }
        }

        /// <summary>
        /// Event which is raised when a digital shelf need to know some stock
        /// </summary>
        public event StockInfoRequestEventHandler StockInfoRequested
        {
            add
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for StockInfoRequested as been registered for '{0}'", value.Method.Name);
                    _stockInfoRequested += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    this.Info("Eventhandler for StockInfoRequested as been unregistered for '{0}'", value.Method.Name);
                    _stockInfoRequested -= value;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RowaDigitalShelf"/> instance.
        /// </summary>
        public RowaDigitalShelf( int subscriberID = RowaDigitalShelf.DefaultSubscriberID )
        {
            LogManagerProxy.Initialize();
            LogVersion();
            InitializeXmlSerializer();
            _messageDispatcher.MessageArrived += OnMessageArrived;
            _messageDispatcher.MessageStreamDown += OnMessageStreamDown;

				this.SubscriberID = subscriberID;
        }

        /// <summary>
        /// Establishes a new connection to the digital shelf with the the specified host at the specified port.
        /// This method performs an implicit disconnect if there is already an active digital shelf connection.
        /// </summary>
        /// <param name="host">The name or ip address of the digital shelf.</param>
        /// <param name="port">The port number of the digital shelf. Default is 6050.</param>
        public void Connect(string host, ushort port = 6052)
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
                this.Trace("Connecting to digital shelf '{0}' at port '{1}'.", host, port);

                _client = new TcpClient();

                var connectResult = _client.BeginConnect(host, (int)port, null, null);

                if (connectResult.AsyncWaitHandle.WaitOne(ConnectTimeout) == false)
                {
                    connectResult.AsyncWaitHandle.Close();
                    _client.Close();
                    _client = null;
                    this.Error("Connecting to digital shelf '{0}' at port '{1}' timed out.", host, port);
                    throw new TimeoutException(string.Format("Connecting to digital shelf '{0}' at port '{1}' timed out.", host, port));
                }

                _client.EndConnect(connectResult);
                _client.ReceiveTimeout = 0;
                _client.IncreaseReadBufferSize();
                _client.EnableSocketKeepAlive();

                // initialize message stream and register the supported XML message types
                _messageObjectStream = new XmlObjectStream(_client.GetStream(), "WWKS", "DigitalShelf", string.Concat(host, ":", port));
                _messageObjectStream.AddSupportedType(typeof(HelloRequestEnvelope), typeof(HelloRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(HelloResponseEnvelope), typeof(HelloResponse).Name);

                _messageObjectStream.AddSupportedType(typeof(ArticleInfoRequestEnvelope), typeof(ArticleInfoRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(ArticleInfoResponseEnvelope), typeof(ArticleInfoResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(ArticlePriceRequestEnvelope), typeof(ArticlePriceRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(ArticlePriceResponseEnvelope), typeof(ArticlePriceResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(ArticleSelectedMessageEnvelope), typeof(ArticleSelectedMessage).Name);

                _messageObjectStream.AddSupportedType(typeof(ShoppingCartRequestEnvelope), typeof(ShoppingCartRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(ShoppingCartResponseEnvelope), typeof(ShoppingCartResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(ShoppingCartUpdateRequestEnvelope), typeof(ShoppingCartUpdateRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(ShoppingCartUpdateResponseEnvelope), typeof(ShoppingCartUpdateResponse).Name);
                _messageObjectStream.AddSupportedType(typeof(ShoppingCartUpdateMessageEnvelope), typeof(ShoppingCartUpdateMessage).Name);

                _messageObjectStream.AddSupportedType(typeof(StockInfoRequestEnvelope), typeof(StockInfoRequest).Name);
                _messageObjectStream.AddSupportedType(typeof(StockInfoResponseEnvelope), typeof(StockInfoResponse).Name);

                if (ProcessHandshake() == false)
                {
                    Disconnect();
                    throw new ApplicationException("Processing WWKS protocol handshake failed.");
                }

                _messageDispatcher.Start(_messageObjectStream);
                _shutdownThreadEvent = new ManualResetEvent(false);

                // TODO: Implement ConnectionCheck.
                //if (ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectionCheck), _shutdownThreadEvent) == false)
                //{
                //    Disconnect();
                //    throw new ApplicationException("Starting background connection check thread failed.");
                //}
            }
        }

        /// <summary>
        /// Performs a graceful shutdown of the digital shelf connection. 
        /// </summary>
        public void Disconnect()
        {
            lock (_syncLock)
            {
                if (_messageObjectStream == null)
                {
                    return;
                }

                this.Trace("Closing connection to digital shelf.");

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
        /// Creates a new shopping cart instance.
        /// </summary>
        /// <param name="id">The unique id of the shopping cart.</param>
        /// <param name="status">The status of the shopping cart.</param>
        /// <param name="customerId">The unique id of the customer the shopping cart belongs to.</param>
        /// <param name="salesPersonId">The unique id of the corresponding sales person.</param>
        /// <param name="salesPointId">The unique id of the corresponding sales point.</param>
        /// <param name="viewPointId">The unique id of the corresponding view point.</param>
        /// <returns>An <see cref="IShoppingCart"/> instance with the specified attributes.</returns>
        public IShoppingCart CreateShoppingCart(string id, ShoppingCartStatus status, string customerId = "", string salesPersonId = "", string salesPointId = "", string viewPointId = "")
        {
            var shoppingCart = new ShoppingCart()
            {
                Id = TextConverter.EscapeInvalidXmlChars(id),
                Status = status.ToString(),
                CustomerId = TextConverter.EscapeInvalidXmlChars(customerId),
                SalesPersonId = TextConverter.EscapeInvalidXmlChars(salesPersonId),
                SalesPointId = TextConverter.EscapeInvalidXmlChars(salesPointId),
                ViewPointId = TextConverter.EscapeInvalidXmlChars(viewPointId),
            };

            return shoppingCart;
        }

        /// <summary>
        /// Notifies the connected digital shelf/shelves when a shopping cart has been manipulated in the It-System.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart that has been manipulated.</param>
        /// <returns><c>true</c> if the shopping cart update message was sent successfully, <c>false</c> otherwise.</returns>
        public bool UpdateShoppingCart(IShoppingCart shoppingCart)
        {
            if (shoppingCart == null)
            {
                throw new ArgumentException("No shopping cart specified.");
            }

            if (!this.Connected)
            {
                throw new InvalidOperationException("The digital shelf is currently not connected.");
            }

            var message = new ShoppingCartUpdateMessageEnvelope()
            {
                ShoppingCartUpdateMessage = new ShoppingCartUpdateMessage()
                {
                    Id = MessageId.Next,
                    Destination = _destinationId,
                    Source = SubscriberID,
                    ShoppingCart = (ShoppingCart)shoppingCart
                }
            };
            
            if (!_messageObjectStream.Write(message))
            {
                throw new ApplicationException("Sending 'ShoppingCartUpdateMessage' to digital shelf failed.");
            }

            return true;
        }

        #endregion

        #region Private Methods

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

                var v3 = new XmlSerializer(typeof(ArticleInfoRequestEnvelope));
                var v4 = new XmlSerializer(typeof(ArticleInfoResponseEnvelope));
                var v5 = new XmlSerializer(typeof(ArticlePriceRequestEnvelope));
                var v6 = new XmlSerializer(typeof(ArticlePriceResponseEnvelope));
                var v7 = new XmlSerializer(typeof(ArticleSelectedMessageEnvelope));

                var v8 = new XmlSerializer(typeof(ShoppingCartRequestEnvelope));
                var v9 = new XmlSerializer(typeof(ShoppingCartResponseEnvelope));
                var v10 = new XmlSerializer(typeof(ShoppingCartUpdateRequestEnvelope));
                var v11 = new XmlSerializer(typeof(ShoppingCartUpdateResponseEnvelope));
                var v12 = new XmlSerializer(typeof(ShoppingCartUpdateMessageEnvelope));

                var v13 = new XmlSerializer(typeof(StockInfoRequestEnvelope));
                var v14 = new XmlSerializer(typeof(StockInfoResponseEnvelope));

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

        /// <summary>
        /// Processes the WWKS 2.0 protocol handshake to initialize the connection to the digital shelf.
        /// </summary>
        private bool ProcessHandshake()
        {
            HelloRequestEnvelope request = new HelloRequestEnvelope()
            {
                HelloRequest = new HelloRequest()
                {
                    Id = MessageId.Next, //TODO: Don't use static message id generator but create instance.
                    Subscriber = new Subscriber()
                    {
                        Id = SubscriberID,
                        Type = SubscriberType.IMS,
                        Manufacturer = "CareFusion Germany 326 GmbH",
                        ProductInfo = "StorageSystem Library",
                        VersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion,
                        Capability = Capabilities
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
                    case "ArticleInfoRequest": ProcessArticleInfoRequest((ArticleInfoRequest)eventArgs.Message); break;
                    case "ArticlePriceRequest": ProcessArticlePriceRequest((ArticlePriceRequest)eventArgs.Message); break;
                    case "ShoppingCartRequest": ProcessShoppingCartRequest((ShoppingCartRequest)eventArgs.Message); break;
                    case "ShoppingCartUpdateRequest": ProcessShoppingCartUpdateRequest((ShoppingCartUpdateRequest)eventArgs.Message); break;
                    case "ArticleSelectedMessage": ProcessArticleSelectedMessage((ArticleSelectedMessage)eventArgs.Message); break;
                    case "StockInfoRequest": ProcessStockInfoRequest((StockInfoRequest)eventArgs.Message); break;
                }
            }
            catch (Exception ex)
            {
                this.Error("Processing incomming message '{0}' failed!", ex, eventArgs.Message.GetType().Name);
            }
        }

        /// <summary>
        /// Processes an incomming article info request.
        /// </summary>
        /// <param name="request">The request to be processed.</param>
        private void ProcessArticleInfoRequest(ArticleInfoRequest request)
        {
            // Set the underlying XmlObjectStream to send the response on when request.Finish() is called.
            lock (_syncLock)
            {
                if (_messageObjectStream == null)
                {
                    return;
                }

                request.MessageObjectStream = _messageObjectStream;
            }

            // Raise the corresponding event.
            base.Raise("ArticleInfoRequested", _articleInfoRequested, this, request);
        }

        /// <summary>
        /// Processes an incomming article price request.
        /// </summary>
        /// <param name="request">The request to be processed.</param>
        private void ProcessArticlePriceRequest(ArticlePriceRequest request)
        {
            // Set the underlying XmlObjectStream to send the response on when request.Finish() is called.
            lock (_syncLock)
            {
                if (_messageObjectStream == null)
                {
                    return;
                }

                request.MessageObjectStream = _messageObjectStream;
            }

            // Raise the corresponding event.
            base.Raise("ArticlePriceRequested", _articlePriceRequested, this, request);
        }

        /// <summary>
        /// Processes an incomming shopping cart request.
        /// </summary>
        /// <param name="request">The request to be processed.</param>
        private void ProcessShoppingCartRequest(ShoppingCartRequest request)
        {
            // Set the underlying XmlObjectStream to send the response on when request.Accept() or request.Reject() is called.
            lock (_syncLock)
            {
                if (_messageObjectStream == null)
                {
                    return;
                }

                request.MessageObjectStream = _messageObjectStream;
            }

            // Raise the corresponding event.
            base.Raise("ShoppingCartRequested", _shoppingCartRequested, this, request);
        }

        /// <summary>
        /// Processes an incomming shopping cart update request.
        /// </summary>
        /// <param name="request">The request to be processed.</param>
        private void ProcessShoppingCartUpdateRequest(ShoppingCartUpdateRequest request)
        {
            // Set the underlying XmlObjectStream to send the response on when request.Accept() or request.Reject() is called.
            lock (_syncLock)
            {
                if (_messageObjectStream == null)
                {
                    return;
                }

                request.MessageObjectStream = _messageObjectStream;
            }

            // Raise the corresponding event.
            base.Raise("ShoppingCartUpdateRequested", _shoppingCartUpdateRequested, this, request);
        }

        /// <summary>
        /// Processes an incomming article selected message.
        /// </summary>
        /// <param name="message">The message to be processed.</param>
        private void ProcessArticleSelectedMessage(ArticleSelectedMessage message)
        {
            // Raise the corresponding event.
            base.Raise("ArticleSelected", _articleSelected, this, message.Article);
        }

        /// <summary>
        /// Processes an incomming Stock Information request.
        /// </summary>
        /// <param name="request">The request to be processed.</param>
        private void ProcessStockInfoRequest(StockInfoRequest request)
        {
            // Set the underlying XmlObjectStream to send the response on when request.Accept() or request.Reject() is called.
            lock (_syncLock)
            {
                if (_messageObjectStream == null)
                {
                    return;
                }

                request.MessageObjectStream = _messageObjectStream;
            }

            // Raise the corresponding event.
            base.Raise("StockInfoRequest", _stockInfoRequested, this, request);
        }

        #endregion
    }
}
