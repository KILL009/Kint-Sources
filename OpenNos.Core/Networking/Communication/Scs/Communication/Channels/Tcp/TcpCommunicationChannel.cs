using OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints;
using OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNos.Core.Networking.Communication.Scs.Communication.Channels.Tcp
{
    /// <summary>
    /// This class is used to communicate with a remote application over TCP/IP protocol.
    /// </summary>
    public class TcpCommunicationChannel : CommunicationChannelBase, IDisposable
    {
        #region Members

        /// <summary>
        /// Size of the buffer that is used to receive bytes from TCP socket.
        /// </summary>
        private const int RECEIVE_BUFFER_SIZE = 4 * 1024;

        /// <summary>
        /// This buffer is used to receive bytes
        /// </summary>
        private readonly byte[] buffer;

        /// <summary>
        /// Socket object to send/reveice messages.
        /// </summary>
        private readonly Socket clientSocket;

        // 4KB
        private readonly ScsTcpEndPoint remoteEndPoint;

        /// <summary>
        /// This object is just used for thread synchronizing (locking).
        /// </summary>
        private readonly object syncLock;

        private bool disposed;
        private ConcurrentQueue<byte[]> highPriorityBuffer;
        private ConcurrentQueue<byte[]> lowPriorityBuffer;
        private Random random = new Random();

        /// <summary>
        /// A flag to control thread's running
        /// </summary>
        private volatile bool running;

        private CancellationTokenSource sendCancellationToken = new CancellationTokenSource();

        private Task sendTask;

        #endregion

        #region Instantiation

        /// <summary>
        /// Creates a new TcpCommunicationChannel object.
        /// </summary>
        /// <param name="clientSocket">
        /// A connected Socket object that is used to communicate over network
        /// </param>
        public TcpCommunicationChannel(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            this.clientSocket.NoDelay = true;

            // initialize lagging mode
            var isLagMode = ConfigurationManager.AppSettings["LagMode"].ToLower() == "true";

            var ipEndPoint = (IPEndPoint)this.clientSocket.RemoteEndPoint;
            remoteEndPoint = new ScsTcpEndPoint(ipEndPoint.Address.ToString(), ipEndPoint.Port);

            buffer = new byte[RECEIVE_BUFFER_SIZE];
            syncLock = new object();

            highPriorityBuffer = new ConcurrentQueue<byte[]>();
            lowPriorityBuffer = new ConcurrentQueue<byte[]>();
            var cancellationToken = sendCancellationToken.Token;
            sendTask = StartSending(SendInterval, new TimeSpan(0, 0, 0, 0, isLagMode ? 1000 : 10), cancellationToken);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the endpoint of remote application.
        /// </summary>
        public override ScsEndPoint RemoteEndPoint => remoteEndPoint;

        #endregion

        #region Methods

        public static async Task StartSending(Action action, TimeSpan period, CancellationToken sendCancellationToken)
        {
            while (!sendCancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, sendCancellationToken);

                if (!sendCancellationToken.IsCancellationRequested)
                {
                    action();
                }
            }
        }

        public override async Task ClearLowPriorityQueue()
        {
            lowPriorityBuffer.Clear();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Disconnects from remote application and closes channel.
        /// </summary>
        public override void Disconnect()
        {
            if (CommunicationState != CommunicationStates.Connected)
            {
                return;
            }

            running = false;
            try
            {
                sendCancellationToken.Cancel();
                if (clientSocket.Connected)
                {
                    clientSocket.Close();
                }

                clientSocket.Dispose();
            }
            catch
            {
                // do nothing
            }
            finally
            {
                sendCancellationToken.Dispose();
            }

            CommunicationState = CommunicationStates.Disconnected;
            OnDisconnected();
        }

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

        public void SendInterval()
        {
            try
            {
                if (WireProtocol != null)
                {
                    SendByPriority(highPriorityBuffer);
                    SendByPriority(lowPriorityBuffer);
                }
            }
            catch (Exception)
            {
                // disconnect
            }

            if (!clientSocket.Connected)
            {
                // do nothing
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
                sendCancellationToken.Dispose();
            }
        }

        /// <summary>
        /// Sends a message to the remote application.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        protected override void SendMessagepublic(IScsMessage message, byte priority)
        {
            if (priority > 5)
            {
                highPriorityBuffer.Enqueue(WireProtocol.GetBytes(message));
            }
            else
            {
                lowPriorityBuffer.Enqueue(WireProtocol.GetBytes(message));
            }
        }

        /// <summary>
        /// Starts the thread to receive messages from socket.
        /// </summary>
        protected override void Startpublic()
        {
            running = true;
            clientSocket.BeginReceive(buffer, 0, buffer.Length, 0, ReceiveCallback, null);
        }

        private static void SendCallback(IAsyncResult result)
        {
            try
            {
                // Retrieve the socket from the state object.
                var client = (Socket)result.AsyncState;

                if (!client.Connected)
                {
                    return;
                }

                // Complete sending the data to the remote device.
                var bytesSent = client.EndSend(result);
            }
            catch (Exception)
            {
                // disconnect
            }
        }

        /// <summary>
        /// This method is used as callback method in _clientSocket's BeginReceive method. It
        /// reveives bytes from socker.
        /// </summary>
        /// <param name="result">Asyncronous call result</param>
        private void ReceiveCallback(IAsyncResult result)
        {
            if (!running)
            {
                return;
            }

            try
            {
                var bytesRead = -1;

                // Get received bytes count
                bytesRead = clientSocket.EndReceive(result);

                if (bytesRead > 0)
                {
                    LastReceivedMessageTime = DateTime.Now;

                    // Copy received bytes to a new byte array
                    byte[] receivedBytes = new byte[bytesRead];
                    Array.Copy(buffer, receivedBytes, bytesRead);

                    // Read messages according to current wire protocol
                    IEnumerable<IScsMessage> messages = WireProtocol.CreateMessages(receivedBytes);

                    // Raise MessageReceived event for all received messages
                    foreach (IScsMessage message in messages)
                        OnMessageReceived(message, DateTime.Now);
                }
                else
                {
                    Logger.Log.Warn(Language.Instance.GetMessageFromKey("CLIENT_DISCONNECTED"));
                    Disconnect();
                }

                // Read more bytes if still running
                if (running)
                {
                    clientSocket.BeginReceive(buffer, 0, buffer.Length, 0, ReceiveCallback, null);
                }
            }
            catch
            {
                Disconnect();
            }
        }

        private void SendByPriority(ConcurrentQueue<byte[]> buffer)
        {
            IEnumerable<byte> outgoingPacket = new List<byte>();

            // send max 30 packets at once
            for (int i = 0; i < 30; i++)
            {
                if (buffer.TryDequeue(out byte[] message) && message != null)
                {
                    outgoingPacket = outgoingPacket.Concat(message);
                }
                else
                {
                    break;
                }
            }

            if (outgoingPacket.Any())
            {
                clientSocket.BeginSend(outgoingPacket.ToArray(), 0, outgoingPacket.Count(), SocketFlags.None,
                SendCallback, clientSocket);
            }
        }

        #endregion
    }
}