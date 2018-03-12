using System.Collections.Concurrent;

namespace OpenNos.GameObject
{
    public class NpcToSummon
    {
        #region Instantiation

        public NpcToSummon(short vnum, MapCell spawnCell, long target, ConcurrentBag<EventContainer> deathEvents, bool isProtected = false, bool isMate = false)
        {
            VNum = vnum;
            SpawnCell = spawnCell;
            Target = target;
            DeathEvents = deathEvents;
            IsProtected = isProtected;
            IsMate = isMate;
        }

        #endregion

        #region Properties

        public ConcurrentBag<EventContainer> DeathEvents { get; }

        public bool IsMate { get; }

        public bool IsProtected { get; }

        public MapCell SpawnCell { get; }

        public long Target { get; }

        public short VNum { get; }

        #endregion
    }
}