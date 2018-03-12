using OpenNos.Domain;
using System;

namespace OpenNos.Data
{
    public class ItemInstanceDTO : SynchronizableBaseDTO, IItemInstance
    {
        #region Properties

        public byte Amount { get; set; }

        public long? BoundCharacterId { get; set; }

        public long CharacterId { get; set; }

        public short Design { get; set; }

        public int DurabilityPoint { get; set; }

        public DateTime? ItemDeleteTime { get; set; }

        public short ItemVNum { get; set; }

        public sbyte Rare { get; set; }

        public short Slot { get; set; }

        public InventoryType Type { get; set; }

        public byte Upgrade { get; set; }

        #endregion
    }
}