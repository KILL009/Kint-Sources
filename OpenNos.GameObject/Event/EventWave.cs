using System;
using System.Collections.Concurrent;

namespace OpenNos.GameObject
{
    public class EventWave
    {
        #region Instantiation

        public EventWave(byte delay, ConcurrentBag<EventContainer> events, byte offset = 0)
        {
            Delay = delay;
            Offset = offset;
            Events = events;
        }

        #endregion

        #region Properties

        public byte Delay { get; private set; }

        public ConcurrentBag<EventContainer> Events { get; private set; }

        public DateTime LastStart { get; set; }

        public byte Offset { get; set; }

        #endregion
    }
}