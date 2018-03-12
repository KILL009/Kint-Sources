using OpenNos.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class FamilyLogDTO : MappingBaseDTO
    {
        #region Properties

        public long FamilyId { get; set; }

        public string FamilyLogData { get; set; }

        [Key]
        public long FamilyLogId { get; set; }

        public FamilyLogType FamilyLogType { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}