using OpenNos.DAL.EF;
using OpenNos.Data;
using System;

namespace OpenNos.Mapper.Mappers
{
    public class ItemInstanceMapper
    {
        public ItemInstanceMapper()
        {

        }

        public bool ToItemInstanceDTO(ItemInstance input, ItemInstanceDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Ammo = input.Ammo ?? 0;
            output.Amount = input.Amount;
            output.BoundCharacterId = input.BoundCharacterId;
            output.Cellon = input.Cellon ?? 0;
            output.CharacterId = input.CharacterId;
            output.CloseDefence = input.CloseDefence ?? 0;
            output.Concentrate = input.Concentrate ?? 0;
            output.CriticalDodge = input.CriticalDodge ?? 0;
            output.CriticalLuckRate = input.CriticalLuckRate ?? 0;
            output.CriticalRate = input.CriticalRate ?? 0;
            output.DamageMaximum = input.DamageMaximum ?? 0;
            output.DamageMinimum = input.DamageMinimum ?? 0;
            output.DarkElement = input.DarkElement ?? 0;
            output.DarkResistance = input.DarkResistance ?? 0;
            output.DefenceDodge = input.DefenceDodge ?? 0;
            output.Design = input.Design;
            output.DistanceDefence = input.DistanceDefence ?? 0;
            output.DistanceDefenceDodge = input.DistanceDefenceDodge ?? 0;
            output.DurabilityPoint = input.DurabilityPoint;
            output.ElementRate = input.ElementRate ?? 0;
            output.EquipmentSerialId = input.EquipmentSerialId ?? Guid.Empty;
            output.FireElement = input.FireElement ?? 0;
            output.FireResistance = input.FireResistance ?? 0;
            output.HitRate = input.HitRate ?? 0;
            output.HoldingVNum = input.HoldingVNum ?? 0;
            output.HP = input.HP ?? 0;
            output.Id = input.Id;
            output.IsEmpty = input.IsEmpty ?? false;
            output.IsFixed = input.IsFixed ?? false;
            output.ItemDeleteTime = input.ItemDeleteTime;
            output.ItemVNum = input.ItemVNum;
            output.LightElement = input.LightElement ?? 0;
            output.LightResistance = input.LightResistance ?? 0;
            output.MagicDefence = input.MagicDefence ?? 0;
            output.MaxElementRate = input.MaxElementRate ?? 0;
            output.MP = input.MP ?? 0;
            output.Rare = (sbyte)input.Rare;
            output.SlDamage = input.SlDamage ?? 0;
            output.SlDefence = input.SlDefence ?? 0;
            output.SlElement = input.SlElement ?? 0;
            output.SlHP = input.SlHP ?? 0;
            output.Slot = input.Slot;
            output.SpDamage = input.SpDamage ?? 0;
            output.SpDark = input.SpDark ?? 0;
            output.SpDefence = input.SpDefence ?? 0;
            output.SpElement = input.SpElement ?? 0;
            output.SpFire = input.SpFire ?? 0;
            output.SpHP = input.SpHP ?? 0;
            output.SpLevel = input.SpLevel ?? 0;
            output.SpLight = input.SpLight ?? 0;
            output.SpStoneUpgrade = input.SpStoneUpgrade ?? 0;
            output.SpWater = input.SpWater ?? 0;
            output.Type = input.Type;
            output.Upgrade = input.Upgrade;
            output.WaterElement = input.WaterElement ?? 0;
            output.WaterResistance = input.WaterResistance ?? 0;
            output.XP = input.XP ?? 0;
            return true;
        }

        public bool ToItemInstance(ItemInstanceDTO input, ItemInstance output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Ammo = input.Ammo;
            output.Amount = input.Amount;
            output.BoundCharacterId = input.BoundCharacterId;
            output.Cellon = input.Cellon;
            output.CharacterId = input.CharacterId;
            output.CloseDefence = input.CloseDefence;
            output.Concentrate = input.Concentrate;
            output.CriticalDodge = input.CriticalDodge;
            output.CriticalLuckRate = input.CriticalLuckRate;
            output.CriticalRate = input.CriticalRate;
            output.DamageMaximum = input.DamageMaximum;
            output.DamageMinimum = input.DamageMinimum;
            output.DarkElement = input.DarkElement;
            output.DarkResistance = input.DarkResistance;
            output.DefenceDodge = input.DefenceDodge;
            output.Design = input.Design;
            output.DistanceDefence = input.DistanceDefence;
            output.DistanceDefenceDodge = input.DistanceDefenceDodge;
            output.DurabilityPoint = input.DurabilityPoint;
            output.ElementRate = input.ElementRate;
            output.EquipmentSerialId = input.EquipmentSerialId;
            output.FireElement = input.FireElement;
            output.FireResistance = input.FireResistance;
            output.HitRate = input.HitRate;
            output.HoldingVNum = input.HoldingVNum;
            output.HP = input.HP;
            output.Id = input.Id;
            output.IsEmpty = input.IsEmpty;
            output.IsFixed = input.IsFixed;
            output.ItemDeleteTime = input.ItemDeleteTime;
            output.ItemVNum = input.ItemVNum;
            output.LightElement = input.LightElement;
            output.LightResistance = input.LightResistance;
            output.MagicDefence = input.MagicDefence;
            output.MaxElementRate = input.MaxElementRate;
            output.MP = input.MP;
            output.Rare = input.Rare;
            output.SlDamage = input.SlDamage;
            output.SlDefence = input.SlDefence;
            output.SlElement = input.SlElement;
            output.SlHP = input.SlHP;
            output.Slot = input.Slot;
            output.SpDamage = input.SpDamage;
            output.SpDark = input.SpDark;
            output.SpDefence = input.SpDefence;
            output.SpElement = input.SpElement;
            output.SpFire = input.SpFire;
            output.SpHP = input.SpHP;
            output.SpLevel = input.SpLevel;
            output.SpLight = input.SpLight;
            output.SpStoneUpgrade = input.SpStoneUpgrade;
            output.SpWater = input.SpWater;
            output.Type = input.Type;
            output.Upgrade = input.Upgrade;
            output.WaterElement = input.WaterElement;
            output.WaterResistance = input.WaterResistance;
            output.XP = input.XP;
            return true;
        }
    }
}
