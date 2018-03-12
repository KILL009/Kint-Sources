using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class LogChat
    {
        #region Properties

        public virtual Character Character { get; set; }

        public long? CharacterId { get; set; }

        [MaxLength(255)]
        public string ChatMessage { get; set; }

        public byte ChatType { get; set; }

        [MaxLength(255)]
        public string IpAddress { get; set; }

        [Key]
        public long LogId { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}