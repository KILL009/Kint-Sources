using System;
namespace OpenNos.Data
{
    public class MinigameLogDTO
    {
        public long MinigameLogId { get; set; }

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public int Score { get; set; }

        public byte Minigame { get; set; }

        public long CharacterId { get; set; }
    }
}
