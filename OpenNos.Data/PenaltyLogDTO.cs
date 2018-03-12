using OpenNos.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class PenaltyLogDTO : MappingBaseDTO
    {
        #region Properties

        public long AccountId { get; set; }

        public string AdminName { get; set; }

        public DateTime DateEnd { get; set; }

        public DateTime DateStart { get; set; }

        public PenaltyType Penalty { get; set; }

        [Key]
        public int PenaltyLogId { get; set; }

        public string Reason { get; set; }

        #endregion
    }
}