using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class LogChatDTO : MappingBaseDTO
    {
        #region Properties

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