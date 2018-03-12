using OpenNos.Domain;
using System;

namespace OpenNos.GameObject
{
    public class Schedule
    {
        #region Properties

        public EventType Event { get; set; }

        public TimeSpan Time { get; set; }

        #endregion
    }
}