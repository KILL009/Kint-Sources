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

using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class Skill : SkillDTO
    {
        #region Instantiation

        public Skill()
        {
            Combos = new List<ComboDTO>();
            BCards = new List<BCard>();
        }

        public Skill(SkillDTO input)
        {
            AttackAnimation = input.AttackAnimation;
            CastAnimation = input.CastAnimation;
            CastEffect = input.CastEffect;
            CastId = input.CastId;
            CastTime = input.CastTime;
            Class = input.Class;
            Cooldown = input.Cooldown;
            CPCost = input.CPCost;
            Duration = input.Duration;
            Effect = input.Effect;
            Element = input.Element;
            HitType = input.HitType;
            ItemVNum = input.ItemVNum;
            Level = input.Level;
            LevelMinimum = input.LevelMinimum;
            MinimumAdventurerLevel = input.MinimumAdventurerLevel;
            MinimumArcherLevel = input.MinimumArcherLevel;
            MinimumMagicianLevel = input.MinimumMagicianLevel;
            MinimumSwordmanLevel = input.MinimumSwordmanLevel;
            MpCost = input.MpCost;
            Name = input.Name;
            Price = input.Price;
            Range = input.Range;
            SkillType = input.SkillType;
            SkillVNum = input.SkillVNum;
            TargetRange = input.TargetRange;
            TargetType = input.TargetType;
            Type = input.Type;
            UpgradeSkill = input.UpgradeSkill;
            UpgradeType = input.UpgradeType;
            Combos = new List<ComboDTO>();
            BCards = new List<BCard>();
        }

        #endregion

        #region Properties

        public List<BCard> BCards { get; set; }

        public List<ComboDTO> Combos { get; set; }

        #endregion
    }
}