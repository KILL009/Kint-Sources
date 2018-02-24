using System;

namespace OpenNos.XMLModel.Objects.Quest
{
    [Serializable]
    public class LootObjective
    {
        #region Properties

        public short Chance { get; set; }

        public int CurrentAmount { get; set; }

        public short[] DroppedByMonsterVNum { get; set; }

        public int GoalAmount { get; set; }

        public short ItemVNum { get; set; }

        #endregion
    }
}