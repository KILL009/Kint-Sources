using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class StaticBuffDTO : MappingBaseDTO
    {
        #region Properties

        public short CardId { get; set; }

        public long CharacterId { get; set; }

        public int RemainingTime { get; set; }

        [Key]
        public long StaticBuffId { get; set; }

        #endregion
    }
}