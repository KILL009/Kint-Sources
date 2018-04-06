using OpenNos.Domain;

/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

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
        public InventoryType Type { get; set; }

        public byte Upgrade { get; set; }

        public short? HoldingVNum { get; set; }

        public short? SlDamage { get; set; }

        public short? SlDefence { get; set; }

        public short? SlElement { get; set; }

        public short? SlHP { get; set; }

        public byte? SpDamage { get; set; }

        public byte? SpDark { get; set; }

        public byte? SpDefence { get; set; }

        public byte? SpElement { get; set; }

        public byte? SpFire { get; set; }

        public byte? SpHP { get; set; }

        public byte? SpLevel { get; set; }

        public byte? SpLight { get; set; }

        public byte? SpStoneUpgrade { get; set; }

        public byte? SpWater { get; set; }

        public byte? Ammo { get; set; }

        public byte? Cellon { get; set; }

        public short? CloseDefence { get; set; }

        public short? Concentrate { get; set; }

        public short? CriticalDodge { get; set; }

        public byte? CriticalLuckRate { get; set; }

        public short? CriticalRate { get; set; }

        public short? DamageMaximum { get; set; }

        public short? DamageMinimum { get; set; }

        public byte? DarkElement { get; set; }

        public short? DarkResistance { get; set; }

        public short? DefenceDodge { get; set; }

        public short? DistanceDefence { get; set; }

        public short? DistanceDefenceDodge { get; set; }

        public short? ElementRate { get; set; }

        public Guid? EquipmentSerialId { get; set; }

        public byte? FireElement { get; set; }

        public short? FireResistance { get; set; }

        public short? HitRate { get; set; }

        public short? HP { get; set; }

        public bool? IsEmpty { get; set; }

        public bool? IsFixed { get; set; }

        public byte? LightElement { get; set; }

        public short? LightResistance { get; set; }

        public short? MagicDefence { get; set; }

        public short? MaxElementRate { get; set; }

        public short? MP { get; set; }

        public byte? WaterElement { get; set; }

        public short? WaterResistance { get; set; }

        public long? XP { get; set; }

        #endregion
    }
}