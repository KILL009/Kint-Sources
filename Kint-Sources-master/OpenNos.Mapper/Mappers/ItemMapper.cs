using OpenNos.DAL.EF;
using OpenNos.Data;
using OpenNos.Domain;

namespace OpenNos.Mapper.Mappers
{
    public class ItemMapper
    {
        public ItemMapper()
        {

        }

        public bool ToItemDTO(Item input, ItemDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.BasicUpgrade = input.BasicUpgrade;
            output.CellonLvl = input.CellonLvl;
            output.Class = input.Class;
            output.CloseDefence = input.CloseDefence;
            output.Color = input.Color;
            output.Concentrate = input.Concentrate;
            output.CriticalLuckRate = input.CriticalLuckRate;
            output.CriticalRate = input.CriticalRate;
            output.DamageMaximum = input.DamageMaximum;
            output.DamageMinimum = input.DamageMinimum;
            output.DarkElement = input.DarkElement;
            output.DarkResistance = input.DarkResistance;
            output.DefenceDodge = input.DefenceDodge;
            output.DistanceDefence = input.DistanceDefence;
            output.DistanceDefenceDodge = input.DistanceDefenceDodge;
            output.Effect = input.Effect;
            output.EffectValue = input.EffectValue;
            output.Element = input.Element;
            output.ElementRate = input.ElementRate;
            output.EquipmentSlot = input.EquipmentSlot;
            output.FireElement = input.FireElement;
            output.FireResistance = input.FireResistance;
            output.Height = input.Height;
            output.HitRate = input.HitRate;
            output.Hp = input.Hp;
            output.HpRegeneration = input.HpRegeneration;
            output.IsBlocked = input.IsBlocked;
            output.IsColored = input.IsColored;
            output.IsConsumable = input.IsConsumable;
            output.IsDroppable = input.IsDroppable;
            output.IsHeroic = input.IsHeroic;
            output.IsPrestige = input.IsPrestige;
            output.IsHolder = input.IsHolder;
            output.IsMinilandObject = input.IsMinilandObject;
            output.IsSoldable = input.IsSoldable;
            output.IsTradable = input.IsTradable;
            output.ItemSubType = input.ItemSubType;
            output.ItemType = (ItemType)input.ItemType;
            output.ItemValidTime = input.ItemValidTime;
            output.LevelJobMinimum = input.LevelJobMinimum;
            output.LevelMinimum = input.LevelMinimum;
            output.LightElement = input.LightElement;
            output.LightResistance = input.LightResistance;
            output.MagicDefence = input.MagicDefence;
            output.MaxCellon = input.MaxCellon;
            output.MaxCellonLvl = input.MaxCellonLvl;
            output.MaxElementRate = input.MaxElementRate;
            output.MaximumAmmo = input.MaximumAmmo;
            output.MinilandObjectPoint = input.MinilandObjectPoint;
            output.MoreHp = input.MoreHp;
            output.MoreMp = input.MoreMp;
            output.Morph = input.Morph;
            output.Mp = input.Mp;
            output.MpRegeneration = input.MpRegeneration;
            output.Name = input.Name;
            output.Price = input.Price;
            output.PvpDefence = input.PvpDefence;
            output.PvpStrength = input.PvpStrength;
            output.ReduceOposantResistance = input.ReduceOposantResistance;
            output.ReputationMinimum = input.ReputationMinimum;
            output.ReputPrice = input.ReputPrice;
            output.SecondaryElement = input.SecondaryElement;
            output.Sex = input.Sex;
            output.Speed = input.Speed;
            output.SpType = input.SpType;
            output.Type = (InventoryType)input.Type;
            output.VNum = input.VNum;
            output.WaitDelay = input.WaitDelay;
            output.WaterElement = input.WaterElement;
            output.WaterResistance = input.WaterResistance;
            output.Width = input.Width;
            return true;
        }

        public bool ToItem(ItemDTO input, Item output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.BasicUpgrade = input.BasicUpgrade;
            output.CellonLvl = input.CellonLvl;
            output.Class = input.Class;
            output.CloseDefence = input.CloseDefence;
            output.Color = input.Color;
            output.Concentrate = input.Concentrate;
            output.CriticalLuckRate = input.CriticalLuckRate;
            output.CriticalRate = input.CriticalRate;
            output.DamageMaximum = input.DamageMaximum;
            output.DamageMinimum = input.DamageMinimum;
            output.DarkElement = input.DarkElement;
            output.DarkResistance = input.DarkResistance;
            output.DefenceDodge = input.DefenceDodge;
            output.DistanceDefence = input.DistanceDefence;
            output.DistanceDefenceDodge = input.DistanceDefenceDodge;
            output.Effect = input.Effect;
            output.EffectValue = input.EffectValue;
            output.Element = input.Element;
            output.ElementRate = input.ElementRate;
            output.EquipmentSlot = input.EquipmentSlot;
            output.FireElement = input.FireElement;
            output.FireResistance = input.FireResistance;
            output.Height = input.Height;
            output.HitRate = input.HitRate;
            output.Hp = input.Hp;
            output.HpRegeneration = input.HpRegeneration;
            output.IsBlocked = input.IsBlocked;
            output.IsColored = input.IsColored;
            output.IsConsumable = input.IsConsumable;
            output.IsDroppable = input.IsDroppable;
            output.IsHeroic = input.IsHeroic;
            output.IsPrestige = input.IsPrestige;
            output.IsHolder = input.IsHolder;
            output.IsMinilandObject = input.IsMinilandObject;
            output.IsSoldable = input.IsSoldable;
            output.IsTradable = input.IsTradable;
            output.ItemSubType = input.ItemSubType;
            output.ItemType = (byte)input.ItemType;
            output.ItemValidTime = input.ItemValidTime;
            output.LevelJobMinimum = input.LevelJobMinimum;
            output.LevelMinimum = input.LevelMinimum;
            output.LightElement = input.LightElement;
            output.LightResistance = input.LightResistance;
            output.MagicDefence = input.MagicDefence;
            output.MaxCellon = input.MaxCellon;
            output.MaxCellonLvl = input.MaxCellonLvl;
            output.MaxElementRate = input.MaxElementRate;
            output.MaximumAmmo = input.MaximumAmmo;
            output.MinilandObjectPoint = input.MinilandObjectPoint;
            output.MoreHp = input.MoreHp;
            output.MoreMp = input.MoreMp;
            output.Morph = input.Morph;
            output.Mp = input.Mp;
            output.MpRegeneration = input.MpRegeneration;
            output.Name = input.Name;
            output.Price = input.Price;
            output.PvpDefence = input.PvpDefence;
            output.PvpStrength = input.PvpStrength;
            output.ReduceOposantResistance = input.ReduceOposantResistance;
            output.ReputationMinimum = input.ReputationMinimum;
            output.ReputPrice = input.ReputPrice;
            output.SecondaryElement = input.SecondaryElement;
            output.Sex = input.Sex;
            output.Speed = input.Speed;
            output.SpType = input.SpType;
            output.Type = (byte)input.Type;
            output.VNum = input.VNum;
            output.WaitDelay = input.WaitDelay;
            output.WaterElement = input.WaterElement;
            output.WaterResistance = input.WaterResistance;
            output.Width = input.Width;
            return true;
        }
    }
}
