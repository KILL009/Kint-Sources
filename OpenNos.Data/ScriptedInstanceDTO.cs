using OpenNos.Domain;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public class ScriptedInstanceDTO : MappingBaseDTO
    {
        #region Properties

        public short MapId { get; set; }

        public string Name { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public string Script { get; set; }

        [Key]
        public short ScriptedInstanceId { get; set; }

        public ScriptedInstanceType Type { get; set; }

        #endregion
    }
}