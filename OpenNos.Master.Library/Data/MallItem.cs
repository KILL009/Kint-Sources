using System;

namespace OpenNos.Master.Library.Data
{
    [Serializable]
    public class MallItem
    {
        public int Amount { get; set; }

        public short ItemVNum { get; set; }

        public byte Rare { get; set; }

        public byte Upgrade { get; set; }
    }
}
