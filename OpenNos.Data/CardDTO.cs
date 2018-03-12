using OpenNos.Domain;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class CardDTO : MappingBaseDTO
    {
        #region Properties

        public BuffType BuffType { get; set; }

        [Key]
        public short CardId { get; set; }

        public int Delay { get; set; }

        public int Duration { get; set; }

        public int EffectId { get; set; }

        public byte Level { get; set; }

        public string Name { get; set; }

        public byte Propability { get; set; }

        public short TimeoutBuff { get; set; }

        public byte TimeoutBuffChance { get; set; }

        #endregion
    }
}