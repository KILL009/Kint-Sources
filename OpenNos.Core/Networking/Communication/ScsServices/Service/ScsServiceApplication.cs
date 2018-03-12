using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messengers;
using OpenNos.Core.Networking.Communication.Scs.Server;
using OpenNos.Core.Networking.Communication.ScsServices.Communication.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace OpenNos.Core.Networking.Communication.ScsServices.Service
{
    /// <summary>
    /// Implements IScsServiceApplication and provides all functionallity.
    /// </summary>
    public class ScsServiceApplication : IScsServiceApplication, IDisposable
    {
        #region Members

        /// <summary>
        /// Underlying IScsServer object to accept and manage client connections.
        /// </summary>
        private readonly IScsServer scsServer;

        /// <summary>
        /// All connected clients to service.
        /// Key: Client's unique Id.
        /// Value: Reference to the client.
        /// </summary>
        private readonly ConcurrentDictionary<long, IScsServiceClient> serviceClients;

        /// <summary>
        /// User service objects that is used to invoke incoming method invocation requests.
        /// Key: Service interface type's name.
        /// Value: Service object.
        /// </summary>
        private readonly ConcurrentDictionary<string, ServiceObject> serviceObjects;

        private bool disposed;

        #endregion

        #region Instantiation

        /// <summary>
        /// Creates a new ScsServiceApplication object.
        /// </summary>
        /// <param name="scsServer">Underlying IScsServer object to accept and manage client connections</param>
        /// <exception cref="ArgumentNullException">
        /// Throws ArgumentNullException if scsServer argument is null
        /// </exception>
        public ScsServiceApplication(IScsServer scsServer)
        {
            this.scsServer = scsServer ?? throw new ArgumentNullException(nameof(scsServer));
            this.scsServer.ClientConnected += ScsServer_ClientConnected;
            this.scsServer.ClientDisconnected += ScsServer_ClientDisconnected;
            serviceObjects = new ConcurrentDictionary<string, ServiceObject>();
            serviceClients = new ConcurrentDictionary<long, IScsServiceClient>();
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is raised when a new client connected to the service.
        /// </summary>
        public event EventHandler<ServiceClientEventArgs> ClientConnected;

        /// <summary>
        /// This event is raised when a client disconnected from the service.
        /// </summary>
        public event EventHandler<ServiceClientEventArgs> ClientDisconnected;

        #endregion

        #region Methods

        /// <summary>
        /// Adds a service object to this service application. Only single service object can be
        /// added for a service interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <typeparam name="TServiceClass">
        /// Service class type. Must be delivered from ScsService and must implement TServiceInterface.
        /// </typeparam>
        /// <param name="service">An instance of TServiceClass.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws ArgumentNullException if service argument is null
        /// </exception>
        /// <exception cref="Exception">Throws Exception if service is already added before</exception>
        public void AddService<TServiceInterface, TServiceClass>(TServiceClass service)
            where TServiceClass : ScsService, TServiceInterface
            where TServiceInterface : class
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var type = typeof(TServiceInterface);
            if (serviceObjects[type.Name] != null)
            {
                throw new Exception("Service '" + type.Name + "' is already added before.");
            }

            serviceObjects[type.Name] = new ServiceObject(type, service);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        /// <summary>
        /// Removes a previously added service object from this service application. It removes
        /// object according to interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <returns>True: removed. False: no service object with this interface</returns>
        public bool RemoveService<TServiceInterface>()
            where TServiceInterface : class
        {
            return serviceObjects.TryRemove(typeof(TServiceInterface).Name, out ServiceObject value);
        }

        /// <summary>
        /// Starts service application.
        /// </summary>
        public void Start() => scsServer.Start();

        /// <summary>
        /// Stops service application.
        /// </summary>
        public void Stop() => scsServer.Stop();

        /// <summary>
        /// Sends response to the remote application that invoked a service method.
        /// </summary>
        /// <param name="client">Client that sent invoke message</param>
        /// <param name="requestMessage">Request message</param>
        /// <param name="returnValue">Return value to send</param>
        /// <param name="exception">Exception to send</param>
        private static void SendInvokeResponse(IMessenger client, IScsMessage requestMessage, object returnValue, ScsRemoteException exception)
        {
            try
            {
                client.SendMessage(
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

        /// <summary>
        /// Handles MessageReceived events of all clients, evaluates each message, finds appropriate
        /// service object and invokes appropriate method.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            // Get RequestReplyMessenger object (sender of event) to get client
            RequestReplyMessenger<IScsServerClient> requestReplyMessenger = (RequestReplyMessenger<IScsServerClient>)sender;

            // Cast message to ScsRemoteInvokeMessage and check it
            var invokeMessage = e.Message as ScsRemoteInvokeMessage;
            if (invokeMessage == null)
            {
                return;
            }

            try
            {
                // Get client object
                var client = serviceClients[requestReplyMessenger.Messenger.ClientId];
                if (client == null)
                {
                    requestReplyMessenger.Messenger.Disconnect();
                    return;
                }

                // Get service object
                var serviceObject = serviceObjects[invokeMessage.ServiceClassName];
                if (serviceObject == null)
                {
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, null, new ScsRemoteException("There is no service with name '" + invokeMessage.ServiceClassName + "'"));
                    return;
                }

                // Invoke method
                try
                {
                    // Set client to service, so user service can get client in service method using
                    // CurrentClient property.
                    object returnValue;
                    serviceObject.Service.CurrentClient = client;
                    try
                    {
                        returnValue = serviceObject.InvokeMethod(invokeMessage.MethodName, invokeMessage.Parameters);
                    }
                    finally
                    {
                        // Set CurrentClient as null since method call completed
                        serviceObject.Service.CurrentClient = null;
                    }

                    // Send method invocation return value to the client
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, returnValue, null);
                }
                catch (TargetInvocationException ex)
                {
                    var innerEx = ex.InnerException;
                    if (innerEx != null)
                    {
                        SendInvokeResponse(requestReplyMessenger, invokeMessage, null, new ScsRemoteException(innerEx.Message + Environment.NewLine + "Service Version: " + serviceObject.ServiceAttribute.Version, innerEx));
                    }
                }
                catch (Exception ex)
                {
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, null, new ScsRemoteException(ex.Message + Environment.NewLine + "Service Version: " + serviceObject.ServiceAttribute.Version, ex));
                }
            }
            catch (Exception ex)
            {
                SendInvokeResponse(requestReplyMessenger, invokeMessage, null, new ScsRemoteException("An error occured during remote service method call.", ex));
            }
        }

        /// <summary>
        /// Raises ClientConnected event.
        /// </summary>
        /// <param name="client"></param>
        private void OnClientConnected(IScsServiceClient client)
        {
            EventHandler<ServiceClientEventArgs> handler = ClientConnected;
            handler?.Invoke(this, new ServiceClientEventArgs(client));
        }

        /// <summary>
        /// Raises ClientDisconnected event.
        /// </summary>
        /// <param name="client"></param>
        private void OnClientDisconnected(IScsServiceClient client)
        {
            EventHandler<ServiceClientEventArgs> handler = ClientDisconnected;
            handler?.Invoke(this, new ServiceClientEventArgs(client));
        }

        /// <summary>
        /// Handles ClientConnected event of _scsServer object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void ScsServer_ClientConnected(object sender, ServerClientEventArgs e)
        {
            RequestReplyMessenger<IScsServerClient> requestReplyMessenger = new RequestReplyMessenger<IScsServerClient>(e.Client);
            requestReplyMessenger.MessageReceived += Client_MessageReceived;
            requestReplyMessenger.Start();

            var serviceClient = ScsServiceClientFactory.CreateServiceClient(e.Client, requestReplyMessenger);
            serviceClients[serviceClient.ClientId] = serviceClient;
            OnClientConnected(serviceClient);
        }

        /// <summary>
        /// Handles ClientDisconnected event of _scsServer object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void ScsServer_ClientDisconnected(object sender, ServerClientEventArgs e)
        {
            var serviceClient = serviceClients[e.Client.ClientId];
            if (serviceClient == null)
            {
                return;
            }

            e.Client.Disconnect();
            serviceClients.TryRemove(e.Client.ClientId, out IScsServiceClient value);
            OnClientDisconnected(serviceClient);
        }

        #endregion

        #region Classes

        /// <summary>
        /// Represents a user service object. It is used to invoke methods on a ScsService object.
        /// </summary>
        private sealed class ServiceObject
        {
            #region Members

            /// <summary>
            /// This collection stores a list of all methods of service object.
            /// Key: Method name
            /// Value: Informations about method.
            /// </summary>
            private readonly SortedList<string, MethodInfo> methods;

            #endregion

            #region Instantiation

            /// <summary>
            /// Creates a new ServiceObject.
            /// </summary>
            /// <param name="serviceInterfaceType">Type of service interface</param>
            /// <param name="service">The service object that is used to invoke methods on</param>
            public ServiceObject(Type serviceInterfaceType, ScsService service)
            {
                Service = service;
                object[] classAttributes = serviceInterfaceType.GetCustomAttributes(typeof(ScsServiceAttribute), true);
                if (classAttributes.Length <= 0)
                {
                    throw new Exception("Service interface (" + serviceInterfaceType.Name + ") must has ScsService attribute.");
                }

                ServiceAttribute = classAttributes[0] as ScsServiceAttribute;
                methods = new SortedList<string, MethodInfo>();
                foreach (MethodInfo methodInfo in serviceInterfaceType.GetMethods())
                    methods.Add(methodInfo.Name, methodInfo);
            }

            #endregion

            #region Properties

            /// <summary>
            /// The service object that is used to invoke methods on.
            /// </summary>
            public ScsService Service { get; private set; }

            /// <summary>
            /// ScsService attribute of Service object's class.
            /// </summary>
            public ScsServiceAttribute ServiceAttribute { get; private set; }

            #endregion

            #region Methods

            /// <summary>
            /// Invokes a method of Service object.
            /// </summary>
            /// <param name="methodName">Name of the method to invoke</param>
            /// <param name="parameters">Parameters of method</param>
            /// <returns>Return value of method</returns>
            public object InvokeMethod(string methodName, params object[] parameters)
            {
                // Check if there is a method with name methodName
                if (!methods.ContainsKey(methodName))
                {
                    throw new Exception("There is not a method with name '" + methodName + "' in service class.");
                }

                // Get method
                var method = methods[methodName];

                // Invoke method and return invoke result
                return method.Invoke(Service, parameters);
            }

            #endregion
        }

        #endregion
    }
}