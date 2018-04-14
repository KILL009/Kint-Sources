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
using OpenNos.DAL;
using OpenNos.DAL.EF.Helpers;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.Handler;
using OpenNos.Master.Library.Client;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace OpenNos.Login
{
    public static class Program
    {
        #region Members

        private static bool _isDebug;

        #endregion

        #region Methods

        public static void Main(string[] args)
        {
            checked
            {
                try
                {
                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                    Console.Title = $"Login Server{(_isDebug ? " Development Environment" : string.Empty)}";

                    bool ignoreStartupMessages = false;
                    foreach (string arg in args)
                    {
                        if (arg == "--nomsg")
                        {
                            ignoreStartupMessages = true;
                        }
                    }

                    // initialize Logger
                    Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));

                    int port = Convert.ToInt32(ConfigurationManager.AppSettings["LoginPort"]);
                    if (!ignoreStartupMessages)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                        string text = $"LOGIN SERVER v{fileVersionInfo.ProductVersion}dev - PORT : {port} by Source# Team";
                        int offset = (Console.WindowWidth / 2) + (text.Length / 2);
                        string separator = new string('=', Console.WindowWidth);
                        Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
                    }

                    // initialize api
                    if (CommunicationServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["MasterAuthKey"]))
                    {
                        Logger.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
                    }

                    // initialize DB
                    if (!DataAccessHelper.Initialize())
                    {
                        Console.ReadKey();
                        return;
                    }

                    Logger.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                    try
                    {
                        // initialize PacketSerialization
                        PacketFactory.Initialize<WalkPacket>();

                        NetworkManager<LoginCryptography> networkManager = new NetworkManager<LoginCryptography>("127.0.0.1", port, typeof(LoginPacketHandler), typeof(LoginCryptography), false);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogEventError("INITIALIZATION_EXCEPTION", "General Error Server", ex);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogEventError("INITIALIZATION_EXCEPTION", "General Error", ex);
                    Console.ReadKey();
                }
            }
        }

        #endregion
    }
}