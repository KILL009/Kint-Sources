using OpenNos.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class StaticBonusDTO : MappingBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public DateTime DateEnd { get; set; }

        [Key]
        public long StaticBonusId { get; set; }

        public StaticBonusType StaticBonusType { get; set; }

        #endregion
    }
}