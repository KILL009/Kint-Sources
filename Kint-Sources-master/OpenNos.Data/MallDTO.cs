using OpenNos.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    [Serializable]
    public class MallDTO : MappingBaseDTO
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public int ItemVnum { get; set; }

        public int Amount { get; set; }

        public int Price { get; set; }

        #endregion
    }

    public class MappingBaseDTO
    {
    }
}
