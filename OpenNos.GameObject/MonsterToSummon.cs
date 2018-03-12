using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class MonsterToSummon
    {
        #region Instantiation

        public MonsterToSummon(short vnum, MapCell spawnCell, long target, bool move, bool isTarget = false, bool isBonus = false, bool isHostile = true, bool isBoss = false)
        {
            VNum = vnum;
            SpawnCell = spawnCell;
            Target = target;
            IsMoving = move;
            IsTarget = isTarget;
            IsBonus = isBonus;
            IsBoss = isBoss;
            IsHostile = isHostile;
            DeathEvents = new List<EventContainer>();
            NoticingEvents = new List<EventContainer>();
        }

        #endregion

        #region Properties

        public List<EventContainer> DeathEvents { get; set; }

        public bool IsBonus { get; set; }

        public bool IsBoss { get; set; }

        public bool IsHostile { get; set; }

        public bool IsMoving { get; set; }

        public bool IsTarget { get; set; }

        public byte NoticeRange { get; internal set; }

        public List<EventContainer> NoticingEvents { get; set; }

        public MapCell SpawnCell { get; set; }

        public long Target { get; set; }

        public short VNum { get; set; }

        #endregion
    }
}