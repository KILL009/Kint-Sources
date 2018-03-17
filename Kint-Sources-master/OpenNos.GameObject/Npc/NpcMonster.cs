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
using System;
using System.Collections.Generic;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class NpcMonster : NpcMonsterDTO
    {
        #region Instantiation

        public NpcMonster()
        {
        }

        public NpcMonster(NpcMonsterDTO input)
        {
            AmountRequired = input.AmountRequired;
            AttackClass = input.AttackClass;
            AttackUpgrade = input.AttackUpgrade;
            BasicArea = input.BasicArea;
            BasicCooldown = input.BasicCooldown;
            BasicRange = input.BasicRange;
            BasicSkill = input.BasicSkill;
            Catch = input.Catch;
            CloseDefence = input.CloseDefence;
            Concentrate = input.Concentrate;
            CriticalChance = input.CriticalChance;
            CriticalRate = input.CriticalRate;
            DamageMaximum = input.DamageMaximum;
            DamageMinimum = input.DamageMinimum;
            DarkResistance = input.DarkResistance;
            DefenceDodge = input.DefenceDodge;
            DefenceUpgrade = input.DefenceUpgrade;
            DistanceDefence = input.DistanceDefence;
            DistanceDefenceDodge = input.DistanceDefenceDodge;
            Element = input.Element;
            ElementRate = input.ElementRate;
            FireResistance = input.FireResistance;
            HeroLevel = input.HeroLevel;
            HeroXp = input.HeroXp;
            IsHostile = input.IsHostile;
            JobXP = input.JobXP;
            Level = input.Level;
            LightResistance = input.LightResistance;
            MagicDefence = input.MagicDefence;
            MaxHP = input.MaxHP;
            MaxMP = input.MaxMP;
            MonsterType = input.MonsterType;
            Name = input.Name;
            NoAggresiveIcon = input.NoAggresiveIcon;
            NoticeRange = input.NoticeRange;
            NpcMonsterVNum = input.NpcMonsterVNum;
            Race = input.Race;
            RaceType = input.RaceType;
            RespawnTime = input.RespawnTime;
            Speed = input.Speed;
            VNumRequired = input.VNumRequired;
            WaterResistance = input.WaterResistance;
            XP = input.XP;
        }

        #endregion

        #region Properties

        public List<BCard> BCards { get; set; }

        public List<DropDTO> Drops { get; set; }

        public short FirstX { get; set; }

        public short FirstY { get; set; }

        public DateTime LastEffect { get; private set; }

        public DateTime LastMove { get; private set; }

        public List<NpcMonsterSkill> Skills { get; set; }

        public List<TeleporterDTO> Teleporters { get; set; }

        #endregion

        #region Methods

        public string GenerateEInfo() => $"e_info 10 {NpcMonsterVNum} {Level} {Element} {AttackClass} {ElementRate} {AttackUpgrade} {DamageMinimum} {DamageMaximum} {Concentrate} {CriticalChance} {CriticalRate} {DefenceUpgrade} {CloseDefence} {DefenceDodge} {DistanceDefence} {DistanceDefenceDodge} {MagicDefence} {FireResistance} {WaterResistance} {LightResistance} {DarkResistance} {MaxHP} {MaxMP} -1 {Name.Replace(' ', '^')}";

        public float GetRes(int skillelement)
        {
            switch (skillelement)
            {
                case 0:
                    return FireResistance / 100;

                case 1:
                    return WaterResistance / 100;

                case 2:
                    return LightResistance / 100;

                case 3:
                    return DarkResistance / 100;

                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Intializes the GameObject, will be injected by AutoMapper after Entity -&gt; GO mapping
        /// </summary>
        public void Initialize()
        {
            Teleporters = ServerManager.Instance.GetTeleportersByNpcVNum(NpcMonsterVNum);
            Drops = ServerManager.Instance.GetDropsByMonsterVNum(NpcMonsterVNum);
            LastEffect = LastMove = DateTime.Now;
            Skills = ServerManager.Instance.GetNpcMonsterSkillsByMonsterVNum(NpcMonsterVNum);
        }

        #endregion
    }
}