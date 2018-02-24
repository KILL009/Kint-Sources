using System;
namespace OpenNos.DAL.EF
{
    public class MinigameLog
    {
        public long MinigameLogId { get; set; }

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public int Score { get; set; }

        public byte Minigame { get; set; }

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }
    }
}
