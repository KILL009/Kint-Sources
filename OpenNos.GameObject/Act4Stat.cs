using System;

namespace OpenNos.GameObject
{
    public class Act4Stat
    {
        #region Members

        private readonly DateTime nextMonth;

        private DateTime latestUpdate;
        private int percentage;

        private short totalTime;

        #endregion

        #region Instantiation

        public Act4Stat()
        {
            var olddate = DateTime.Now.AddMonths(1);
            nextMonth = new DateTime(olddate.Year, olddate.Month, 1, 0, 0, 0, olddate.Kind);
            latestUpdate = DateTime.Now;
        }

        #endregion

        #region Properties

        public short CurrentTime
        {
            get { return Mode == 0 ? (short)0 : (short)(latestUpdate.AddSeconds(totalTime) - DateTime.Now).TotalSeconds; }
        }

        public bool IsBerios { get; set; }

        public bool IsCalvina { get; set; }

        public bool IsHatus { get; set; }

        public bool IsMorcos { get; set; }

        public int MinutesUntilReset => (int)(nextMonth - DateTime.Now).TotalMinutes;

        public byte Mode { get; set; }

        public int Percentage
        {
            get { return Mode == 0 ? percentage : 0; }
            set { percentage = value; }
        }

        public short TotalTime
        {
            get { return Mode == 0 ? (short)0 : totalTime; }
            set
            {
                latestUpdate = DateTime.Now;
                totalTime = value;
            }
        }

        #endregion
    }
}