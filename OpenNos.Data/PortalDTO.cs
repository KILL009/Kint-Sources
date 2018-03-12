using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class PortalDTO : MappingBaseDTO
    {
        #region Properties

        public short DestinationMapId { get; set; }

        public short DestinationX { get; set; }

        public short DestinationY { get; set; }

        public bool IsDisabled { get; set; }

        [Key]
        public int PortalId { get; set; }

        public short SourceMapId { get; set; }

        public short SourceX { get; set; }

        public short SourceY { get; set; }

        public short Type { get; set; }

        #endregion
    }
}