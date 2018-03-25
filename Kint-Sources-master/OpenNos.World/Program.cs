/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using log4net;
using OpenNos.Core;
using OpenNos.DAL.EF.Helpers;
using OpenNos.GameObject;
using OpenNos.Handler;
using OpenNos.Master.Library.Client;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Data;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using OpenNos.ChatLog.Networking;
using OpenNos.DAL;
using OpenNos.Data;
using System.Linq;
using System.Net;


namespace OpenNos.World
{
    public static class Program
    {
        #region Members

        private static readonly ManualResetEvent _run = new ManualResetEvent(true);

        private static EventHandler _exitHandler;

        private static bool _isDebug;
       

        #endregion

        #region Delegates

        public delegate bool EventHandler(CtrlType sig);

        #endregion

        #region Enums

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        #endregion

        #region Methods

        public static void Main(string[] args)
        {
#if DEBUG
            _isDebug = true;
            Thread.Sleep(1000);
#endif
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            Console.Title = $"World Server{(_isDebug ? " Development Environment" : string.Empty)}";

            string isA4 = string.Empty;

            bool ignoreStartupMessages = false;
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "--nomsg":
                        ignoreStartupMessages = true;
                        break;
                    case "--act4":
                        isA4 = "Y";
                        break;
                }
            }
           

            // initialize Logger
            Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));

            
            // En: No longer required after --act4 argument implementation but you can re-enable it (optional)
                //  Non più richiesto dopo l'implementazione dell'argomento --act4 ma si può riabilitare (opzionale)
             
            while (isA4 != "Y" && isA4 != "n")
            {
                Console.Write("Do you want to run this channel as Act4 mode? (Y/n): ");
                isA4 = Console.ReadLine();

            }
             
             

            int port;

            if (isA4 == "Y")
            {
                port = Convert.ToInt32(ConfigurationManager.AppSettings["Act4Port"]);
            }
            else
            {
                port = Convert.ToInt32(ConfigurationManager.AppSettings["WorldPort"]);
            }

            if (!ignoreStartupMessages)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string text = $"WORLD SERVER v{fileVersionInfo.ProductVersion}dev - ";

                if (isA4 == "Y")
                {
                    text += $"ACT4 MODE";
                }
                else
                {
                    text += $"PORT : {port}";
                }

                text += " by Source# Team";
                int offset = (Console.WindowWidth / 2) + (text.Length / 2);
                string separator = new string('=', Console.WindowWidth);
                Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
            }

            // initialize api
            string authKey = ConfigurationManager.AppSettings["MasterAuthKey"];
            if (CommunicationServiceClient.Instance.Authenticate(authKey))
            {
                Logger.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
            }

            // initialize DB
            if (DataAccessHelper.Initialize())
            {
                // initialilize maps
                ServerManager.Instance.Initialize();
            }
            else
            {
                Console.ReadKey();
                return;
            }

            // TODO: initialize ClientLinkManager initialize PacketSerialization
            PacketFactory.Initialize<WalkPacket>();

            try
            {
                _exitHandler += exitHandler;
                AppDomain.CurrentDomain.UnhandledException += unhandledExceptionHandler;
                NativeMethods.SetConsoleCtrlHandler(_exitHandler, true);
            }
            catch (Exception ex)
            {
                Logger.Error("General Error", ex);
            }
            NetworkManager<WorldCryptography> networkManager = null;
        portloop:
            try
            {
                networkManager = new NetworkManager<WorldCryptography>(ConfigurationManager.AppSettings["IPAddress"], port, typeof(CommandPacketHandler), typeof(LoginCryptography), true);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    port++;
                    Logger.Info("Port already in use! Incrementing...");
                    goto portloop;
                }
                Logger.Error("General Error", ex);
                Environment.Exit(ex.ErrorCode);
            }

            ServerManager.Instance.ServerGroup = ConfigurationManager.AppSettings["ServerGroup"];
            int sessionLimit = 100; // Needs workaround
            int? newChannelId = CommunicationServiceClient.Instance.RegisterWorldServer(new SerializableWorldServer(ServerManager.Instance.WorldId, ConfigurationManager.AppSettings["IPAddress"], port, sessionLimit, ServerManager.Instance.ServerGroup));

            if (newChannelId.HasValue)
            {
                ServerManager.Instance.ChannelId = newChannelId.Value;
                MailServiceClient.Instance.Authenticate(authKey, ServerManager.Instance.WorldId);
                ConfigurationServiceClient.Instance.Authenticate(authKey, ServerManager.Instance.WorldId);
                ServerManager.Instance.Configuration = ConfigurationServiceClient.Instance.GetConfigurationObject();
                if (ServerManager.Instance.Configuration.UseChatLogService)
                {
                    ChatLogServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["ChatLogKey"]);
                }
                ServerManager.Instance.MallApi = new GameObject.Helpers.MallAPIHelper(ServerManager.Instance.Configuration.MallBaseURL);
            }
            else
            {
                Logger.Error("Could not retrieve ChannelId from Web API.");
                Console.ReadKey();
            }
        }

        private static bool exitHandler(CtrlType sig)
        {
            string serverGroup = ConfigurationManager.AppSettings["ServerGroup"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["WorldPort"]);
            CommunicationServiceClient.Instance.UnregisterWorldServer(ServerManager.Instance.WorldId);

            ServerManager.Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 5));
            ServerManager.Instance.SaveAll();

            Thread.Sleep(5000);
            return false;
        }

        private static void unhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            ServerManager.Instance.InShutdown = true;
            Logger.Error((Exception)e.ExceptionObject);

            Logger.Debug("Server crashed! Rebooting gracefully...");
            string serverGroup = ConfigurationManager.AppSettings["ServerGroup"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["WorldPort"]);
            CommunicationServiceClient.Instance.UnregisterWorldServer(ServerManager.Instance.WorldId);

            ServerManager.Shout(string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 5));
            ServerManager.Instance.SaveAll();

            Process.Start("OpenNos.World.exe", "--nomsg");
            Environment.Exit(1);
        }

        #endregion

        #region Classes

        public static class NativeMethods
        {
            #region Methods

            [DllImport("Kernel32")]
            internal static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

            #endregion
        }

        #endregion
    }
}