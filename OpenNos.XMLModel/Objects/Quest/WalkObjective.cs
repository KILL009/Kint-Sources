using System;

namespace OpenNos.XMLModel.Objects.Quest
{
    [Serializable]
    public class WalkObjective
    {
        #region Properties

        public bool Finished { get; set; }

        public bool HiddenGoal { get; set; }

        public short MapId { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        #endregion
    }
}