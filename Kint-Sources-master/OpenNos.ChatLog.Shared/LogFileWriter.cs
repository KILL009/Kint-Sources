using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.ChatLog.Shared
{
    public class LogFileWriter
    {
        public void WriteLogFile(string path, List<ChatLogEntry> logs)
        {
            if (logs.Count > 0)
            {
                using (FileStream stream = File.Create(path))
                {
                    using (BinaryWriter bw = new BinaryWriter(stream))
                    {
                        bw.Write((byte)0x4F);
                        bw.Write((byte)0x4E);
                        bw.Write((byte)0x43);
                        bw.Write((byte)1);
                        bw.Write(logs.Count);
                        foreach (ChatLogEntry log in logs)
                        {
                            bw.Write(log.Timestamp.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
                            bw.Write((byte)log.MessageType);
                            bw.Write(log.Sender ?? string.Empty);
                            bw.Write(log.SenderId ?? 0);
                            bw.Write(log.Receiver ?? string.Empty);
                            bw.Write(log.ReceiverId ?? 0);
                            bw.Write(log.Message ?? string.Empty);
                        }
                    }
                }
            }
        }
    }
}
