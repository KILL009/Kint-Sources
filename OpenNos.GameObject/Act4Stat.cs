using System;

namespace OpenNos.GameObject
{
    public class Act4Stat
    {
        #region Members

        private DateTime _latestUpdate;
        private DateTime _nextMonth;

        private int _percentage;

        private short _totalTime;

        #endregion

        #region Instantiation

        public Act4Stat()
        {
            DateTime olddate = DateTime.Now.AddMonths(1);
            _nextMonth = new DateTime(olddate.Year, olddate.Month, 1, 0, 0, 0, olddate.Kind);
            _latestUpdate = DateTime.Now;
        }

        #endregion

        #region Properties

        public short CurrentTime => Mode == 0 ? (short)0 : (short)(_latestUpdate.AddSeconds(_totalTime) - DateTime.Now).TotalSeconds;

        public bool IsBerios { get; set; }

        public bool IsCalvina { get; set; }

        public bool IsHatus { get; set; }

        public bool IsMorcos { get; set; }

        public int MinutesUntilReset => (int)(_nextMonth - DateTime.Now).TotalMinutes;

        public byte Mode { get; set; }

        public int Percentage
        {
            get => Mode == 0 ? _percentage : 0;
            set => _percentage = value;
        }

        public short TotalTime
        {
            get => Mode == 0 ? (short)0 : _totalTime;
            set
            {
                _latestUpdate = DateTime.Now;
                _totalTime = value;
            }
        }

        #endregion
    }
}