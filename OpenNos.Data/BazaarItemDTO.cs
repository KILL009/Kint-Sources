using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class BazaarItemDTO : MappingBaseDTO
    {
        #region Properties

        public byte Amount { get; set; }

        [Key]
        public long BazaarItemId { get; set; }

        public DateTime DateStart { get; set; }

        public short Duration { get; set; }

        public bool IsPackage { get; set; }

        public Guid ItemInstanceId { get; set; }

        public bool MedalUsed { get; set; }

        public long Price { get; set; }

        public long SellerId { get; set; }

        #endregion
    }
}