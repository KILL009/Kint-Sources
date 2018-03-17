using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.ChatLog.Shared
{
    public class LogFileReader
    {
        public List<ChatLogEntry> ReadLogFile(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    string header = $"{br.ReadChar()}{br.ReadChar()}{br.ReadChar()}";
                    byte version = br.ReadByte();
                    if (header == "ONC")
                    {
                        switch (version)
                        {
                            case 1:
                                return ReadVersion(br);
                            default:
                                throw new InvalidDataException("File Version invalid!");
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("File Header invalid!");
                    }
                }
            }
        }

        private List<ChatLogEntry> ReadVersion(BinaryReader reader)
        {
            List<ChatLogEntry> result = new List<ChatLogEntry>();
            int count = reader.ReadInt32();
            while (count != 0)
            {
                DateTime timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(reader.ReadDouble());
                ChatLogType chatLogType = (ChatLogType)reader.ReadByte();
                string sender = reader.ReadString();
                long senderId = reader.ReadInt64();
                string receiver = reader.ReadString();
                long receiverId = reader.ReadInt64();
                string message = reader.ReadString();
                result.Add(new ChatLogEntry()
                {
                    Timestamp = timestamp,
                    MessageType = chatLogType,
                    Sender = sender,
                    SenderId = senderId,
                    Receiver = receiver,
                    ReceiverId = receiverId,
                    Message = message
                });
                count--;
            }
            return result;
        }
    }
}
