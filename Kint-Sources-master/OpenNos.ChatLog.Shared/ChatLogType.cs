using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.ChatLog.Shared
{
    public enum ChatLogType : byte
    {
        Map = 0,
        Speaker = 1,
        Whisper = 2,
        BuddyTalk = 3,
        Group = 4,
        Family = 5
    }
}
