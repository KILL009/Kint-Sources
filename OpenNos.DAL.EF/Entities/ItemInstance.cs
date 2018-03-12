using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenNos.DAL.EF
{
    public class ItemInstance : SynchronizableBaseEntity
    {
        #region Instantiation

        public ItemInstance()
        {
            BazaarItem = new HashSet<BazaarItem>();
            MinilandObject = new HashSet<MinilandObject>();
        }

        #endregion

        #region Properties

        public int Amount { get; set; }

        public virtual ICollection<BazaarItem> BazaarItem { get; set; }

        public long? BazaarItemId { get; set; }

        [ForeignKey(nameof(BoundCharacterId))]
        public Character BoundCharacter { get; set; }

        public long? BoundCharacterId { get; set; }

        public virtual Character Character { get; set; }

        [Index("IX_SlotAndType", 1, IsUnique = false, Order = 0)]
        public long CharacterId { get; set; }

        public short Design { get; set; }

        public int DurabilityPoint { get; set; }

        public virtual Item Item { get; set; }

        public DateTime? ItemDeleteTime { get; set; }

        public short ItemVNum { get; set; }

        public virtual ICollection<MinilandObject> MinilandObject { get; set; }

        public short Rare { get; set; }

        [Index("IX_SlotAndType", 2, IsUnique = false, Order = 1)]
        public short Slot { get; set; }

        [Index("IX_SlotAndType", 3, IsUnique = false, Order = 2)]
        public byte Type { get; set; }

        public byte Upgrade { get; set; }

        #endregion
    }
}