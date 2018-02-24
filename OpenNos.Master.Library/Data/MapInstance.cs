using OpenNos.Domain;
using System;

namespace OpenNos.Master.Library.Data
{
    [Serializable]
    public class MapInstance
    {
        public Guid MapInstanceId { get; set; }

        public MapInstanceType Type { get; set; }

        public short MapId { get; set; }

        public short XLength { get; set; }

        public short YLength { get; set; }
    }
}
