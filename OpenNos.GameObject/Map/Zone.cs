using System.Collections.Concurrent;

namespace OpenNos.GameObject
{
    public class ZoneEvent
    {
        #region Instantiation

        public ZoneEvent()
        {
            Events = new ConcurrentBag<EventContainer>();
            Range = 1;
        }

        #endregion

        #region Properties

        public ConcurrentBag<EventContainer> Events { get; set; }

        public short Range { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        #endregion

        #region Methods

        public bool InZone(short positionX, short positionY)
        {
            return positionX <= X + Range && positionX >= X - Range && positionY <= Y + Range && positionY >= Y - Range;
        }

        #endregion
    }
}