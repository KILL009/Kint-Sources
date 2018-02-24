using System;

namespace OpenNos.Data
{
    [Serializable]
    public class QuestProgressDTO
    {
        public long QuestProgressId { get; set; }

        public long QuestId { get; set; }

        public string QuestData { get; set; }

        public long CharacterId { get; set; }

        public bool IsFinished { get; set; }
    }
}
