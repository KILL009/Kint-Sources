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

using OpenNos.DAL;
using OpenNos.Data;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public abstract class Item : ItemDTO
    {
        #region Instantiation

        protected Item()
        {
        }

        protected Item(ItemDTO item) => InitializeItem(item);

        #endregion

        #region Properties

        public List<BCard> BCards { get; set; }

        public List<RollGeneratedItemDTO> RollGeneratedItems { get; set; }

        #endregion

        #region Methods

        public void InitializeItem(ItemDTO input)
        {
            BasicUpgrade = input.BasicUpgrade;
            CellonLvl = input.CellonLvl;
            Class = input.Class;
            CloseDefence = input.CloseDefence;
            Color = input.Color;
            Concentrate = input.Concentrate;
            CriticalLuckRate = input.CriticalLuckRate;
            CriticalRate = input.CriticalRate;
            DamageMaximum = input.DamageMaximum;
            DamageMinimum = input.DamageMinimum;
            DarkElement = input.DarkElement;
            DarkResistance = input.DarkResistance;
            DefenceDodge = input.DefenceDodge;
            DistanceDefence = input.DistanceDefence;
            DistanceDefenceDodge = input.DistanceDefenceDodge;
            Effect = input.Effect;
            EffectValue = input.EffectValue;
            Element = input.Element;
            ElementRate = input.ElementRate;
            EquipmentSlot = input.EquipmentSlot;
            FireElement = input.FireElement;
            FireResistance = input.FireResistance;
            Height = input.Height;
            HitRate = input.HitRate;
            Hp = input.Hp;
            HpRegeneration = input.HpRegeneration;
            IsBlocked = input.IsBlocked;
            IsColored = input.IsColored;
            IsConsumable = input.IsConsumable;
            IsDroppable = input.IsDroppable;
            IsHeroic = input.IsHeroic;
            IsHolder = input.IsHolder;
            IsMinilandObject = input.IsMinilandObject;
            IsSoldable = input.IsSoldable;
            IsTradable = input.IsTradable;
            ItemSubType = input.ItemSubType;
            ItemType = input.ItemType;
            ItemValidTime = input.ItemValidTime;
            LevelJobMinimum = input.LevelJobMinimum;
            LevelMinimum = input.LevelMinimum;
            LightElement = input.LightElement;
            LightResistance = input.LightResistance;
            MagicDefence = input.MagicDefence;
            MaxCellon = input.MaxCellon;
            MaxCellonLvl = input.MaxCellonLvl;
            MaxElementRate = input.MaxElementRate;
            MaximumAmmo = input.MaximumAmmo;
            MinilandObjectPoint = input.MinilandObjectPoint;
            MoreHp = input.MoreHp;
            MoreMp = input.MoreMp;
            Morph = input.Morph;
            Mp = input.Mp;
            MpRegeneration = input.MpRegeneration;
            Name = input.Name;
            Price = input.Price;
            PvpDefence = input.PvpDefence;
            PvpStrength = input.PvpStrength;
            ReduceOposantResistance = input.ReduceOposantResistance;
            ReputationMinimum = input.ReputationMinimum;
            ReputPrice = input.ReputPrice;
            SecondaryElement = input.SecondaryElement;
            Sex = input.Sex;
            Speed = input.Speed;
            SpType = input.SpType;
            Type = input.Type;
            VNum = input.VNum;
            WaitDelay = input.WaitDelay;
            WaterElement = input.WaterElement;
            WaterResistance = input.WaterResistance;
            Width = input.Width;
            BCards = new List<BCard>();
            DAOFactory.BCardDAO.LoadByItemVNum(input.VNum).ToList().ForEach(o => BCards.Add(new BCard(o)));
            RollGeneratedItems = DAOFactory.RollGeneratedItemDAO.LoadByItemVNum(input.VNum).ToList();
        }

        //TODO: Convert to PacketDefinition
        public abstract void Use(ClientSession session, ref ItemInstance inv, byte Option = 0, string[] packetsplit = null);

        #endregion
    }
}