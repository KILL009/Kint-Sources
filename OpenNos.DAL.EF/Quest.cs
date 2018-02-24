using System.Collections.Generic;

namespace OpenNos.DAL.EF
{
    public class Quest
    {
        #region Instantiation

        public Quest() => QuestProgress = new HashSet<QuestProgress>();

        #endregion

        #region Properties

        public string QuestData { get; set; }

        public long QuestId { get; set; }

        public virtual ICollection<QuestProgress> QuestProgress { get; set; }

        #endregion
    }
}