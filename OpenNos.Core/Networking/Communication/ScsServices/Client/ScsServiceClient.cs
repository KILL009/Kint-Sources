using OpenNos.Core.Networking.Communication.Scs.Client;
using OpenNos.Core.Networking.Communication.Scs.Communication;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messengers;
using OpenNos.Core.Networking.Communication.ScsServices.Communication;
using OpenNos.Core.Networking.Communication.ScsServices.Communication.Messages;
using System;
using System.Reflection;

namespace OpenNos.Core.Networking.Communication.ScsServices.Client
{
    /// <summary>
    /// Represents a service client that consumes a SCS service.
    /// </summary>
    /// <typeparam name="T">Type of service interface</typeparam>
    public class ScsServiceClient<T> : IScsServiceClient<T> where T : class
    {
        #region Members

        /// <summary>
        /// Underlying IScsClient object to communicate with server.
        /// </summary>
        private readonly IScsClient client;

        /// <summary>
        /// The client object that is used to call method invokes in client side. May be null if
        /// client has no methods to be invoked by server.
        /// </summary>
        private readonly object clientObject;

        /// <summary>
        /// This object is used to create a transparent proxy to invoke remote methods on server.
        /// </summary>
        private readonly AutoConnectRemoteInvokeProxy<T, IScsClient> realServiceProxy;

        /// <summary>
        /// Messenger object to send/receive messages over _client.
        /// </summary>
        private readonly RequestReplyMessenger<IScsClient> requestReplyMessenger;

        private bool disposed;

        #endregion

        #region Instantiation

        /// <summary>
        /// Creates a new ScsServiceClient object.
        /// </summary>
        /// <param name="client">Underlying IScsClient object to communicate with server</param>
        /// <param name="clientObject">
        /// The client object that is used to call method invokes in client side. May be null if
        /// client has no methods to be invoked by server.
        /// </param>
        public ScsServiceClient(IScsClient client, object clientObject)
        {
            this.client = client;
            this.clientObject = clientObject;

            this.client.Connected += Client_Connected;
            this.client.Disconnected += Client_Disconnected;

            requestReplyMessenger = new RequestReplyMessenger<IScsClient>(client);
            requestReplyMessenger.MessageReceived += RequestReplyMessenger_MessageReceived;

            realServiceProxy = new AutoConnectRemoteInvokeProxy<T, IScsClient>(requestReplyMessenger, this);
            ServiceProxy = (T)realServiceProxy.GetTransparentProxy();
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is raised when client connected to server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// This event is raised when client disconnected from server.
        /// </summary>
        public event EventHandler Disconnected;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current communication state.
        /// </summary>
        public CommunicationStates CommunicationState => client.CommunicationState;

        /// <summary>
        /// Timeout for connecting to a server (as milliseconds). Default value: 15 seconds (15000 ms).
        /// </summary>
        public int ConnectTimeout
        {
            get
            {
                return client.ConnectTimeout;
            }
            set
            {
                client.ConnectTimeout = value;
            }
        }

        /// <summary>
        /// Reference to the service proxy to invoke remote service methods.
        /// </summary>
        public T ServiceProxy { get; private set; }

        /// <summary>
        /// Timeout value when invoking a service method. If timeout occurs before end of remote
        /// method call, an exception is thrown. Use -1 for no timeout (wait indefinite). Default
        /// value: 60000 (1 minute).
        /// </summary>
        public int Timeout
        {
            get
            {
                return requestReplyMessenger.Timeout;
            }
            set
            {
                requestReplyMessenger.Timeout = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Connects to server.
        /// </summary>
        public void Connect() => client.Connect();

        /// <summary>
        /// Disconnects from server. Does nothing if already disconnected.
        /// </summary>
        public void Disconnect() => client.Disconnect();

        /// <summary>
        /// Calls Disconnect method.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Handles Connected event of _client object.
        /// </summary>
        /// <param name="sender">Source of object</param>
        /// <param name="e">Event arguments</param>
        private void Client_Connected(object sender, EventArgs e)
        {
            requestReplyMessenger.Start();
            OnConnected();
        }

        /// <summary>
        /// Handles Disconnected event of _client object.
        /// </summary>
        /// <param name="sender">Source of object</param>
        /// <param name="e">Event arguments</param>
        private void Client_Disconnected(object sender, EventArgs e)
        {
            requestReplyMessenger.Stop();
            OnDisconnected();
        }

        /// <summary>
        /// Raises Connected event.
        /// </summary>
        private void OnConnected()
        {
            var handler = Connected;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises Disconnected event.
        /// </summary>
        private void OnDisconnected()
        {
            var handler = Disconnected;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles MessageReceived event of messenger. It gets messages from server and invokes
        /// appropriate method.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void RequestReplyMessenger_MessageReceived(object sender, MessageEventArgs e)
        {
            // Cast message to ScsRemoteInvokeMessage and check it
            var invokeMessage = e.Message as ScsRemoteInvokeMessage;
            if (invokeMessage == null)
            {
                return;
            }

            // Check client object.
            if (clientObject == null)
            {
                SendInvokeResponse(invokeMessage, null, new ScsRemoteException("Client does not wait for method invocations by server."));
                return;
            }

            // Invoke method
            object returnValue;
            try
            {
                var type = clientObject.GetType();
                var method = type.GetMethod(invokeMessage.MethodName);
                returnValue = method.Invoke(clientObject, invokeMessage.Parameters);
            }
            catch (TargetInvocationException ex)
            {
                var innerEx = ex.InnerException;
                if (innerEx != null)
                {
                    SendInvokeResponse(invokeMessage, null, new ScsRemoteException(innerEx.Message, innerEx));
                }

                return;
            }
            catch (Exception ex)
            {
                SendInvokeResponse(invokeMessage, null, new ScsRemoteException(ex.Message, ex));
                return;
            }

            // Send return value
            SendInvokeResponse(invokeMessage, returnValue, null);
        }

        /// <summary>
        /// Sends response to the remote application that invoked a service method.
        /// </summary>
        /// <param name="requestMessage">Request message</param>
        /// <param name="returnValue">Return value to send</param>
        /// <param name="exception">Exception to send</param>
        private void SendInvokeResponse(IScsMessage requestMessage, object returnValue, ScsRemoteException exception)
        {
            try
            {
                requestReplyMessenger.SendMessage(
                    new ScsRemoteInvokeReturnMessage
                    {
                        RepliedMessageId = requestMessage.MessageId,
                        ReturnValue = returnValue,
                        RemoteException = exception
                    }, 10);
            }
            catch
            {
            }
        }

        #endregion
    }
}