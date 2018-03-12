using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.GameObject.Helpers
{
    public class LogHelper : Singleton<LogHelper>
    {
        #region Members

        private ConcurrentBag<LogChatDTO> logChat = new ConcurrentBag<LogChatDTO>();
        private ConcurrentBag<LogCommandsDTO> logCommands = new ConcurrentBag<LogCommandsDTO>();

        #endregion

        #region Methods

        public void Flush()
        {
            List<LogChatDTO> logch = logChat.ToList();
            List<LogCommandsDTO> logcom = logCommands.ToList();
            logChat.Clear();
            logCommands.Clear();
            DAOFactory.LogChatDAO.InsertOrUpdate(logch);
            DAOFactory.LogCommandsDAO.InsertOrUpdate(logcom);
        }

        public void InsertChatLog(ChatType type, long characterId, string message, string ipAddress)
        {
            var log = new LogChatDTO
            {
                CharacterId = characterId,
                ChatMessage = message,
                IpAddress = ipAddress,
                ChatType = (byte)type,
                Timestamp = DateTime.Now
            };
            logChat.Add(log);
        }

        public void InsertCommandLog(long characterId, PacketDefinition commandPacket, string ipAddress)
        {
            var withoutHeaderpacket = string.Empty;
            string[] packet = commandPacket.OriginalContent.Split(' ');
            for (int i = 1; i < packet.Length; i++)
                withoutHeaderpacket += $" {packet[i]}";

            var command = new LogCommandsDTO
            {
                CharacterId = characterId,
                Command = commandPacket.OriginalHeader,
                Data = withoutHeaderpacket,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
            logCommands.Add(command);
        }

        #endregion
    }
}