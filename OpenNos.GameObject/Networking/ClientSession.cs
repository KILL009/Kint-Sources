using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using OpenNos.Domain;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace OpenNos.GameObject
{
    public class ClientSession
    {
        #region Members

        public bool HealthStop;

        private static EncryptionBase encryptor;
        private readonly INetworkClient client;
        private readonly ConcurrentQueue<byte[]> receiveQueue;
        private readonly IList<string> waitForPacketList = new List<string>();
        private Character character;
        private IDictionary<string, HandlerMethodReference> handlerMethods;

        // private byte countPacketReceived;
        private long lastPacketReceive;

        private Random random;
        private object receiveQueueObservable;

        // Packetwait Packets
        private int? waitForPacketsAmount;

        #endregion

        #region Instantiation

        public ClientSession(INetworkClient client)
        {
            // set last received
            lastPacketReceive = DateTime.Now.Ticks;

            // lag mode
            random = new Random((int)client.ClientId);

            // initialize lagging mode
            var isLagMode = ConfigurationManager.AppSettings["LagMode"].ToLower() == "true";

            // initialize network client
            this.client = client;

            // absolutely new instantiated Client has no SessionId
            SessionId = 0;

            // register for NetworkClient events
            this.client.MessageReceived += OnNetworkClientMessageReceived;

            // start observer for receiving packets
            receiveQueue = new ConcurrentQueue<byte[]>();
            receiveQueueObservable = Observable.Interval(new TimeSpan(0, 0, 0, 0, isLagMode ? 1000 : 10)).Subscribe(x => HandlePackets());
        }

        #endregion

        #region Properties

        public Account Account { get; private set; }

        public Character Character
        {
            get
            {
                if (character == null || !HasSelectedCharacter)
                {
                    // cant access an
                    Logger.Log.Warn("Uninitialized Character cannot be accessed.");
                }

                return character;
            }

            private set
            {
                character = value;
            }
        }

        public long ClientId => client.ClientId;

        public MapInstance CurrentMapInstance { get; set; }

        public IDictionary<string, HandlerMethodReference> HandlerMethods
        {
            get
            {
                return handlerMethods ?? (handlerMethods = new Dictionary<string, HandlerMethodReference>());
            }

            set
            {
                handlerMethods = value;
            }
        }

        public bool HasCurrentMapInstance => CurrentMapInstance != null;

        public bool HasSelectedCharacter { get; set; }

        public bool HasSession => client != null;

        public string IpAddress => client.IpAddress.Contains("tcp://") ? client.IpAddress.Replace("tcp://", "") : client.IpAddress;

        public bool IsAuthenticated { get; set; }

        public bool IsConnected => client.IsConnected;

        public bool IsDisposing
        {
            get
            {
                return client.IsDisposing;
            }

            set
            {
                client.IsDisposing = value;
            }
        }

        public bool IsLocalhost => IpAddress.Contains("127.0.0.1");

        public bool IsOnMap => CurrentMapInstance != null;

        public int LastKeepAliveIdentity { get; set; }

        public DateTime RegisterTime { get; internal set; }

        public int SessionId { get; set; }

        #endregion

        #region Methods

        public void ClearLowPriorityQueue() => client.ClearLowPriorityQueue();

        public void Destroy()
        {
            // unregister from events
            CommunicationServiceClient.Instance.CharacterConnectedEvent -= OnOtherCharacterConnected;
            CommunicationServiceClient.Instance.CharacterDisconnectedEvent -= OnOtherCharacterDisconnected;

            // do everything necessary before removing client, DB save, Whatever
            if (HasSelectedCharacter)
            {
                Character.Dispose();
                if (Character.MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance || Character.MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                {
                    Character.MapInstance.InstanceBag.DeadList.Add(Character.CharacterId);
                    if (Character.MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        Character?.Group?.Characters.ToList().ForEach(s =>
                        {
                            s.SendPacket(s.Character.Group.GeneraterRaidmbf());
                            s.SendPacket(s.Character.Group.GenerateRdlst());
                        });
                    }
                }

                if (Character?.Miniland != null)
                {
                    ServerManager.Instance.RemoveMapInstance(Character.Miniland.MapInstanceId);
                }

                // TODO Check why ExchangeInfo.TargetCharacterId is null Character.CloseTrade();
                // disconnect client
                CommunicationServiceClient.Instance.DisconnectCharacter(ServerManager.Instance.WorldId, Character.CharacterId);

                // unregister from map if registered
                if (CurrentMapInstance != null)
                {
                    CurrentMapInstance.UnregisterSession(Character.CharacterId);
                    CurrentMapInstance = null;
                    ServerManager.Instance.UnregisterSession(Character.CharacterId);
                }
            }

            if (Account != null)
            {
                CommunicationServiceClient.Instance.DisconnectAccount(Account.AccountId);
            }

            ClearReceiveQueue();
        }

        public void Disconnect() => client.Disconnect();

        public string GenerateIdentity() => $"Account: {Account.Name}";

        public void Initialize(EncryptionBase encryptor, Type packetHandler, bool isWorldServer)
        {
            ClientSession.encryptor = encryptor;
            client.Initialize(encryptor);

            // dynamically create packethandler references
            GenerateHandlerReferences(packetHandler, isWorldServer);
        }

        public void InitializeAccount(Account account, bool crossServer = false)
        {
            Account = account;
            if (crossServer)
            {
                CommunicationServiceClient.Instance.ConnectAccountInternal(ServerManager.Instance.WorldId, account.AccountId, SessionId);
            }
            else
            {
                CommunicationServiceClient.Instance.ConnectAccount(ServerManager.Instance.WorldId, account.AccountId, SessionId);
            }

            IsAuthenticated = true;
        }

        // [Obsolete("Primitive string operations will be removed in future, use PacketDefinition
        // SendPacket instead. SendPacket with string parameter should only be used for debugging.")]
        public void SendPacket(string packet, byte priority = 10)
        {
            if (!IsDisposing)
            {
                client.SendPacket(packet, priority);
            }
        }

        public void SendPacket(PacketDefinition packet, byte priority = 10)
        {
            if (!IsDisposing)
            {
                client.SendPacket(PacketFactory.Serialize(packet), priority);
            }
        }

        public void SendPacketAfter(string packet, int milliseconds)
        {
            if (!IsDisposing)
            {
                Observable.Timer(TimeSpan.FromMilliseconds(milliseconds)).Subscribe(o =>
                {
                    SendPacket(packet);
                });
            }
        }

        public void SendPacketFormat(string packet, params object[] param)
        {
            if (!IsDisposing)
            {
                client.SendPacketFormat(packet, param);
            }
        }

        // [Obsolete("Primitive string operations will be removed in future, use PacketDefinition
        // SendPacket instead. SendPacket with string parameter should only be used for debugging.")]
        public void SendPackets(IEnumerable<string> packets, byte priority = 10)
        {
            if (!IsDisposing)
            {
                client.SendPackets(packets, priority);
            }
        }

        public void SendPackets(IEnumerable<PacketDefinition> packets, byte priority = 10)
        {
            if (!IsDisposing)
            {
                packets.ToList().ForEach(s => client.SendPacket(PacketFactory.Serialize(s), priority));
            }
        }

        public void SetCharacter(Character character)
        {
            Character = character;

            // register events
            CommunicationServiceClient.Instance.CharacterConnectedEvent += OnOtherCharacterConnected;
            CommunicationServiceClient.Instance.CharacterDisconnectedEvent += OnOtherCharacterDisconnected;

            HasSelectedCharacter = true;

            // register for servermanager
            ServerManager.Instance.RegisterSession(this);
            Character.SetSession(this);
            Character.Buff = new ConcurrentBag<Buff>();
        }

        private void ClearReceiveQueue()
        {
            while (receiveQueue.TryDequeue(out byte[] outPacket))
            {
                // do nothing
            }
        }

        private void GenerateHandlerReferences(Type type, bool isWorldServer)
        {
            IEnumerable<Type> handlerTypes = !isWorldServer ? type.Assembly.GetTypes().Where(t => t.Name.Equals("LoginPacketHandler")) // shitty but it works, reflection?
                                                            : type.Assembly.GetTypes().Where(p =>
                                                            {
                                                                var interfaceType = type.GetInterfaces().FirstOrDefault();
                                                                return interfaceType != null && !p.IsInterface && interfaceType.IsAssignableFrom(p);
                                                            });

            // iterate thru each type in the given assembly
            foreach (Type handlerType in handlerTypes)
            {
                var handler = (IPacketHandler)Activator.CreateInstance(handlerType, this);

                // include PacketDefinition
                foreach (MethodInfo methodInfo in handlerType.GetMethods().Where(x => x.GetCustomAttributes(false).OfType<PacketAttribute>().Any() || x.GetParameters().FirstOrDefault()?.ParameterType.BaseType == typeof(PacketDefinition)))
                {
                    List<PacketAttribute> packetAttributes = methodInfo.GetCustomAttributes(false).OfType<PacketAttribute>().ToList();

                    // assume PacketDefinition based handler method
                    if (!packetAttributes.Any())
                    {
                        var methodReference = new HandlerMethodReference(DelegateBuilder.BuildDelegate<Action<object, object>>(methodInfo), handler, methodInfo.GetParameters().FirstOrDefault()?.ParameterType);
                        HandlerMethods.Add(methodReference.Identification, methodReference);
                    }
                    else
                    {
                        // assume string based handler method
                        foreach (PacketAttribute packetAttribute in packetAttributes)
                        {
                            var methodReference = new HandlerMethodReference(DelegateBuilder.BuildDelegate<Action<object, object>>(methodInfo), handler, packetAttribute);
                            HandlerMethods.Add(methodReference.Identification, methodReference);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle the packet received by the Client.
        /// </summary>
        private void HandlePackets()
        {
            while (receiveQueue.TryDequeue(out byte[] packetData))
            {
                // determine first packet
                if (encryptor.HasCustomParameter && SessionId == 0)
                {
                    var sessionPacket = encryptor.DecryptCustomParameter(packetData);

                    string[] sessionParts = sessionPacket.Split(' ');
                    if (sessionParts.Length == 0)
                    {
                        return;
                    }

                    if (!int.TryParse(sessionParts[0], out int lastka))
                    {
                        Disconnect();
                    }

                    LastKeepAliveIdentity = lastka;

                    // set the SessionId if Session Packet arrives
                    if (sessionParts.Length < 2)
                    {
                        return;
                    }

                    if (!int.TryParse(sessionParts[1].Split('\\').FirstOrDefault(), out int sessid))
                    {
                        return;
                    }

                    SessionId = sessid;
                    Logger.Log.DebugFormat(Language.Instance.GetMessageFromKey("CLIENT_ARRIVED"), SessionId);

                    if (!waitForPacketsAmount.HasValue)
                    {
                        TriggerHandler("OpenNos.EntryPoint", string.Empty, false);
                    }

                    return;
                }

                var packetConcatenated = encryptor.Decrypt(packetData, SessionId);

                foreach (string packet in packetConcatenated.Split(new[] { (char)0xFF }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var packetstring = packet.Replace('^', ' ');
                    string[] packetsplit = packetstring.Split(' ');

                    if (encryptor.HasCustomParameter)
                    {
                        // keep alive
                        var nextKeepAliveRaw = packetsplit[0];
                        if (!int.TryParse(nextKeepAliveRaw, out int nextKeepaliveIdentity) && nextKeepaliveIdentity != LastKeepAliveIdentity + 1)
                        {
                            Logger.Log.ErrorFormat(Language.Instance.GetMessageFromKey("CORRUPTED_KEEPALIVE"), client.ClientId);
                            client.Disconnect();
                            return;
                        }

                        if (nextKeepaliveIdentity == 0)
                        {
                            if (LastKeepAliveIdentity == ushort.MaxValue)
                            {
                                LastKeepAliveIdentity = nextKeepaliveIdentity;
                            }
                        }
                        else
                        {
                            LastKeepAliveIdentity = nextKeepaliveIdentity;
                        }

                        if (waitForPacketsAmount.HasValue)
                        {
                            waitForPacketList.Add(packetstring);
                            string[] packetssplit = packetstring.Split(' ');
                            if (packetssplit.Length > 3 && packetsplit[1] == "DAC")
                            {
                                waitForPacketList.Add("0 CrossServerAuthenticate");
                            }

                            if (waitForPacketList.Count != waitForPacketsAmount)
                            {
                                continue;
                            }

                            waitForPacketsAmount = null;
                            var queuedPackets = string.Join(" ", waitForPacketList.ToArray());
                            var header = queuedPackets.Split(' ', '^')[1];
                            TriggerHandler(header, queuedPackets, true);
                            waitForPacketList.Clear();
                            return;
                        }

                        if (packetsplit.Length <= 1)
                        {
                            continue;
                        }

                        if (packetsplit[1].Length >= 1 && (packetsplit[1][0] == '/' || packetsplit[1][0] == ':' || packetsplit[1][0] == ';'))
                        {
                            packetsplit[1] = packetsplit[1][0].ToString();
                            packetstring = packet.Insert(packet.IndexOf(' ') + 2, " ");
                        }

                        if (packetsplit[1] != "0")
                        {
                            TriggerHandler(packetsplit[1].Replace("#", ""), packetstring, false);
                        }
                    }
                    else
                    {
                        var packetHeader = packetstring.Split(' ')[0];
                        if (string.IsNullOrWhiteSpace(packetHeader))
                        {
                            Disconnect();
                            return;
                        }

                        // simple messaging
                        if (packetHeader[0] == '/' || packetHeader[0] == ':' || packetHeader[0] == ';')
                        {
                            packetHeader = packetHeader[0].ToString();
                            packetstring = packet.Insert(packet.IndexOf(' ') + 2, " ");
                        }

                        TriggerHandler(packetHeader.Replace("#", ""), packetstring, false);
                    }
                }
            }
        }

        /// <summary>
        /// This will be triggered when the underlying NetworkClient receives a packet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNetworkClientMessageReceived(object sender, MessageEventArgs e)
        {
            if (!(e.Message is ScsRawDataMessage message))
            {
                return;
            }

            if (message.MessageData.Any() && message.MessageData.Length > 2)
            {
                receiveQueue.Enqueue(message.MessageData);
            }

            lastPacketReceive = e.ReceivedTimestamp.Ticks;
        }

        private void OnOtherCharacterConnected(object sender, EventArgs e)
        {
            Tuple<long, string> loggedInCharacter = (Tuple<long, string>)sender;

            if (Character.IsFriendOfCharacter(loggedInCharacter.Item1))
            {
                if (Character != null && Character.CharacterId != loggedInCharacter.Item1)
                {
                    client.SendPacket(Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("CHARACTER_LOGGED_IN"), loggedInCharacter.Item2), 10));
                    client.SendPacket(Character.GenerateFinfo(loggedInCharacter.Item1, true));
                }
            }

            var chara = Character.Family?.FamilyCharacters.FirstOrDefault(s => s.CharacterId == loggedInCharacter.Item1);
            if (chara != null && loggedInCharacter.Item1 != Character?.CharacterId)
            {
                client.SendPacket(Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("CHARACTER_FAMILY_LOGGED_IN"), loggedInCharacter.Item2, Language.Instance.GetMessageFromKey(chara.Authority.ToString().ToUpper())), 10));
            }
        }

        private void OnOtherCharacterDisconnected(object sender, EventArgs e)
        {
            Tuple<long, string> loggedOutCharacter = (Tuple<long, string>)sender;
            if (!Character.IsFriendOfCharacter(loggedOutCharacter.Item1))
            {
                return;
            }

            if (Character == null || Character.CharacterId == loggedOutCharacter.Item1)
            {
                return;
            }

            client.SendPacket(Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("CHARACTER_LOGGED_OUT"), loggedOutCharacter.Item2), 10));
            client.SendPacket(Character.GenerateFinfo(loggedOutCharacter.Item1, false));
        }

        private void TriggerHandler(string packetHeader, string packet, bool force)
        {
            if (ServerManager.Instance.InShutdown)
            {
                return;
            }

            if (!IsDisposing)
            {
                var methodReference = HandlerMethods.ContainsKey(packetHeader) ? HandlerMethods[packetHeader] : null;
                if (methodReference != null)
                {
                    if (methodReference.HandlerMethodAttribute != null && !force && methodReference.HandlerMethodAttribute.Amount > 1 && !waitForPacketsAmount.HasValue)
                    {
                        // we need to wait for more
                        waitForPacketsAmount = methodReference.HandlerMethodAttribute.Amount;
                        waitForPacketList.Add(packet != string.Empty ? packet : $"1 {packetHeader} ");
                        return;
                    }

                    try
                    {
                        if (!HasSelectedCharacter && methodReference.ParentHandler.GetType().Name != "CharacterScreenPacketHandler" &&
                            methodReference.ParentHandler.GetType().Name != "LoginPacketHandler")
                        {
                            return;
                        }

                        // call actual handler method
                        if (methodReference.PacketDefinitionParameterType != null)
                        {
                            // check for the correct authority
                            if (IsAuthenticated && (byte)methodReference.Authority > (byte)Account.Authority)
                            {
                                return;
                            }

                            object deserializedPacket = PacketFactory.Deserialize(packet, methodReference.PacketDefinitionParameterType, IsAuthenticated);

                            if (deserializedPacket != null || methodReference.PassNonParseablePacket)
                            {
                                methodReference.HandlerMethod(methodReference.ParentHandler, deserializedPacket);
                            }
                            else
                            {
                                Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("CORRUPT_PACKET"), packetHeader, packet);
                            }
                        }
                        else
                        {
                            methodReference.HandlerMethod(methodReference.ParentHandler, packet);
                        }
                    }
                    catch (DivideByZeroException ex)
                    {
                        // disconnect if something unexpected happens
                        Logger.Log.Error("Handler Error SessionId: " + SessionId, ex);
                        Disconnect();
                    }
                }
                else
                {
                    Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("HANDLER_NOT_FOUND"), packetHeader);
                }
            }
            else
            {
                Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("CLIENTSESSION_DISPOSING"), packetHeader);
            }
        }

        #endregion
    }
}