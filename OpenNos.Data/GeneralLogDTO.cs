using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class GeneralLogDTO : MappingBaseDTO
    {
        #region Properties

        public long? AccountId { get; set; }

        public long? CharacterId { get; set; }

        public string IpAddress { get; set; }

        public string LogData { get; set; }

        [Key]
        public long LogId { get; set; }

        public string LogType { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}