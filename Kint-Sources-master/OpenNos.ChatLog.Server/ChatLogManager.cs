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

using OpenNos.ChatLog.Shared;
using OpenNos.Core;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.ChatLog.Server
{
    internal class ChatLogManager
    {
        #region Members

        private static ChatLogManager _instance;

        private readonly LogFileReader _reader;

        #endregion

        #region Instantiation

        public ChatLogManager()
        {
            _reader = new LogFileReader();
            AuthentificatedClients = new List<long>();
            ChatLogs = new ThreadSafeGenericList<ChatLogEntry>();
            AllChatLogs = new ThreadSafeGenericList<ChatLogEntry>();
            recursiveFileOpen("chatlogs");
            AuthentificationServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["AuthentificationServiceAuthKey"]);
            Observable.Interval(TimeSpan.FromMinutes(15)).Subscribe(observer => SaveChatLogs());
        }

        #endregion

        #region Properties

        public static ChatLogManager Instance => _instance ?? (_instance = new ChatLogManager());

        public List<long> AuthentificatedClients { get; set; }

        public ThreadSafeGenericList<ChatLogEntry> ChatLogs { get; set; }

        public ThreadSafeGenericList<ChatLogEntry> AllChatLogs { get; set; }

        #endregion

        private void SaveChatLogs()
        {
            try
            {
                LogFileWriter writer = new LogFileWriter();
                Logger.Info(Language.Instance.GetMessageFromKey("SAVE_CHATLOGS"));
                List<ChatLogEntry> tmp = ChatLogs.GetAllItems();
                ChatLogs.Clear();
                DateTime current = DateTime.Now;

                string path = "chatlogs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Year.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Month.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, current.Day.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                writer.WriteLogFile(Path.Combine(path, $"{(current.Hour < 10 ? $"0{current.Hour}" : $"{current.Hour}")}.{(current.Minute < 10 ? $"0{current.Minute}" : $"{current.Minute}")}.onc"), tmp);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void recursiveFileOpen(string dir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    foreach (string s in Directory.GetFiles(d).Where(s => s.EndsWith(".onc")))
                    {
                        AllChatLogs.AddRange(_reader.ReadLogFile(s));
                    }
                    recursiveFileOpen(d);
                }
            }
            catch
            {
                Logger.LogEventError("LogFileRead", "Something went wrong while opening Chat Log Files. Exiting...");
                Environment.Exit(-1);
            }
        }
    }
}