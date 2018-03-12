using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenNos.DAL.EF
{
    public class MapTypeMap
    {
        #region Properties

        public virtual Map Map { get; set; }

        [Key, Column(Order = 0)]
        public short MapId { get; set; }

        public virtual MapType MapType { get; set; }

        [Key, Column(Order = 1)]
        public short MapTypeId { get; set; }

        #endregion
    }
}