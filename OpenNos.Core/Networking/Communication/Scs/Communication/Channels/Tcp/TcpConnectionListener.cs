using OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints.Tcp;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OpenNos.Core.Networking.Communication.Scs.Communication.Channels.Tcp
{
    /// <summary>
    /// This class is used to listen and accept incoming TCP connection requests on a TCP port.
    /// </summary>
    public class TcpConnectionListener : ConnectionListenerBase
    {
        #region Members

        /// <summary>
        /// The endpoint address of the server to listen incoming connections.
        /// </summary>
        private readonly ScsTcpEndPoint endPoint;

        /// <summary>
        /// Server socket to listen incoming connection requests.
        /// </summary>
        private TcpListener listenerSocket;

        /// <summary>
        /// A flag to control thread's running
        /// </summary>
        private volatile bool running;

        /// <summary>
        /// The thread to listen socket
        /// </summary>
        private Thread thread;

        #endregion

        #region Instantiation

        /// <summary>
        /// Creates a new TcpConnectionListener for given endpoint.
        /// </summary>
        /// <param name="endPoint">The endpoint address of the server to listen incoming connections</param>
        public TcpConnectionListener(ScsTcpEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts listening incoming connections.
        /// </summary>
        public override void Start()
        {
            StartSocket();
            running = true;
            thread = new Thread(DoListenAsThread);
            thread.Start();
        }

        /// <summary>
        /// Stops listening incoming connections.
        /// </summary>
        public override void Stop()
        {
            running = false;
            StopSocket();
        }

        /// <summary>
        /// Entrance point of the thread. This method is used by the thread to listen incoming requests.
        /// </summary>
        private void DoListenAsThread()
        {
            while (running)
            {
                try
                {
                    var clientSocket = listenerSocket.AcceptSocket();
                    if (clientSocket.Connected)
                    {
                        OnCommunicationChannelConnected(new TcpCommunicationChannel(clientSocket));
                    }
                }
                catch
                {
                    // Disconnect, wait for a while and connect again.
                    StopSocket();
                    Thread.Sleep(1000);
                    if (!running)
                    {
                        return;
                    }

                    try
                    {
                        StartSocket();
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Starts listening socket.
        /// </summary>
        private void StartSocket()
        {
            listenerSocket = new TcpListener(IPAddress.Any, endPoint.TcpPort);
            listenerSocket.Start();
        }

        /// <summary>
        /// Stops listening socket.
        /// </summary>
        private void StopSocket()
        {
            try
            {
                listenerSocket.Stop();
            }
            catch
            {
            }
        }

        #endregion
    }
}