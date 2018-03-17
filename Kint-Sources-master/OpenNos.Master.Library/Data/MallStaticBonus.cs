using OpenNos.Domain;
using System;

namespace OpenNos.Master.Library.Data
{
    [Serializable]
    public class MallStaticBonus
    {
        public StaticBonusType StaticBonus { get; set; }

        public int Seconds { get; set; }
    }
}
