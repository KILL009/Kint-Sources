using Hik.Communication.ScsServices.Service;
using OpenNos.ChatLog.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.ChatLog.Networking
{
    [ScsService(Version = "1.1.0.0")]
    public interface IChatLogService
    {
        /// <summary>
        /// Authenticates a Client to the Service
        /// </summary>
        /// <param name="authKey">The private Authentication key</param>
        /// <returns>true if successful, else false</returns>
        bool Authenticate(string authKey);

        /// <summary>
        /// Authenticates a Client to the Service
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passHash"></param>
        /// <returns></returns>
        bool AuthenticateAdmin(string user, string passHash);

        /// <summary>
        /// Log Chat Message to Chat Log Server
        /// </summary>
        /// <param name="logEntry"></param>
        void LogChatMessage(ChatLogEntry logEntry);

        /// <summary>
        /// Receive Log Entries from Chat Log Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="senderid"></param>
        /// <param name="receiver"></param>
        /// <param name="receiverid"></param>
        /// <param name="message"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        List<ChatLogEntry> GetChatLogEntries(string sender, long? senderid, string receiver, long? receiverid, string message, DateTime? start, DateTime? end, ChatLogType? logType);
    }
}
