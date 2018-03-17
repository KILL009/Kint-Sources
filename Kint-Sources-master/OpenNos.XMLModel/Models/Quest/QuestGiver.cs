using OpenNos.Domain;
using System;

namespace OpenNos.XMLModel.Models.Quest
{
    [Serializable]
    public class QuestGiver
    {
        public QuestGiverType Type { get; set; }

        public long QuestGiverId { get; set; }

        public byte MinimumLevel { get; set; }

        public byte MaximumLevel { get; set; }
    }
}
