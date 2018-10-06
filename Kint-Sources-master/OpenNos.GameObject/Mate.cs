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

using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using OpenNos.GameObject.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.PathFinder;
using static OpenNos.Domain.BCardType;

namespace OpenNos.GameObject
{
    public class Mate : MateDTO
    {
        #region Members

        private NpcMonster _monster;

        private bool _noAttack;
        private bool _noMove;
        private Character _owner;
        public readonly object PveLockObject;

        #endregion

        #region Instantiation

        public Mate(MateDTO input)
        {
            PveLockObject = new object();
            Buff = new ThreadSafeSortedList<short, Buff>();
            Attack = input.Attack;
            CanPickUp = input.CanPickUp;
            CharacterId = input.CharacterId;
            Defence = input.Defence;
            Direction = input.Direction;
            Experience = input.Experience;
            Hp = input.Hp;
            IsSummonable = input.IsSummonable;
            IsTeamMember = input.IsTeamMember;
            Level = input.Level;
            Loyalty = input.Loyalty;
            MapX = input.MapX;
            MapY = input.MapY;
            MateId = input.MateId;
            MateType = input.MateType;
            Mp = input.Mp;
            Name = input.Name;
            NpcMonsterVNum = input.NpcMonsterVNum;
            Skin = input.Skin;
            GenerateMateTransportId();
        }

        public Mate(Character owner, NpcMonster npcMonster, byte level, MateType matetype)
        {
            PveLockObject = new object();
            Buff = new ThreadSafeSortedList<short, Buff>();
            NpcMonsterVNum = npcMonster.NpcMonsterVNum;
            Monster = npcMonster;
            Level = level;
            Hp = MaxHp;
            Mp = MaxMp;
            Name = npcMonster.Name;
            MateType = matetype;
            Loyalty = 1000;
            PositionY = (short)(owner.PositionY + 1);
            PositionX = (short)(owner.PositionX + 1);
            MapX = (short)(owner.PositionX + 1);
            MapY = (short)(owner.PositionY + 1);
            Direction = 2;
            CharacterId = owner.CharacterId;
            Owner = owner;
            GenerateMateTransportId();
        }

        #endregion

        #region Properties

        public BattleEntity BattleEntity { get; set; }

        public ItemInstance ArmorInstance { get; set; }

        public ItemInstance BootsInstance { get; set; }

        public ThreadSafeSortedList<short, Buff> Buff { get; }

        public int BaseDamage => BaseDamageLoad();

        public ItemInstance GlovesInstance { get; set; }

        public bool IsSitting { get; set; }

        public bool IsUsingSp { get; set; }

        public DateTime LastSpeedChange { get; set; }

        public DateTime LastSkillUse { get; set; }

        public int MagicalDefense => MagicalDefenseLoad();

        public int MateTransportId { get; private set; }

        public int MaxHp => HpLoad();

        public int MaxMp => MpLoad();

        public int MeleeDefense => MeleeDefenseLoad();

        public int MeleeDefenseDodge => MeleeDefenseDodgeLoad();

        public NpcMonster Monster
        {
            get => _monster ?? ServerManager.GetNpc(NpcMonsterVNum);

            set => _monster = value;
        }

        public Character Owner
        {
            get => _owner ?? ServerManager.Instance.GetSessionByCharacterId(CharacterId)?.Character;
            set => _owner = value;
        }

        public byte PetId { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public int RangeDefense => RangeDefenseLoad();

        public int RangeDefenseDodge => RangeDefenseDodgeLoad();

        public Skill[] Skills { get; set; }

        public byte Speed
        {
            get
            {
                byte bonusSpeed = (byte)(GetBuff(CardType.Move, (byte)AdditionalTypes.Move.SetMovementNegated)[0]
                                          + GetBuff(CardType.Move,
                                              (byte)AdditionalTypes.Move.MovementSpeedIncreased)[0]
                                          + GetBuff(CardType.Move,
                                              (byte)AdditionalTypes.Move.MovementSpeedDecreased)[0]);

                if (Monster.Speed + bonusSpeed > 59)
                {
                    return 59;
                }

                return (byte)(Monster.Speed + bonusSpeed);
            }
            set
            {
                LastSpeedChange = DateTime.Now;
                Monster.Speed = value > 59 ? (byte)59 : value;
            }
        }

        public ItemInstance SpInstance { get; set; }

        public ItemInstance WeaponInstance { get; set; }
        public DateTime LastMonsterAggro { get; set; }
        public Node[][] BrushFireJagged { get; set; }
        public IEnumerable MapInstance { get;  set; }
        public MapInstance GetMatesInRang { get;  set; }
    #endregion

    #region Methods

    public void AddBuff(Buff indicator)
        {
            if (indicator?.Card != null)
            {
                Buff[indicator.Card.CardId] = indicator;
                indicator.RemainingTime = indicator.Card.Duration;
                indicator.Start = DateTime.Now;

                indicator.Card.BCards.ForEach(c => c.ApplyBCards(this));
                Observable.Timer(TimeSpan.FromMilliseconds(indicator.Card.Duration * 100)).Subscribe(o =>
                {
                    RemoveBuff(indicator.Card.CardId);
                    if (indicator.Card.TimeoutBuff != 0
                        && ServerManager.RandomNumber() < indicator.Card.TimeoutBuffChance)
                    {
                        AddBuff(new Buff(indicator.Card.TimeoutBuff, Monster.Level));
                    }
                });
                _noAttack |= indicator.Card.BCards.Any(s =>
                    s.Type == (byte)CardType.SpecialAttack
                    && s.SubType.Equals((byte)AdditionalTypes.SpecialAttack.NoAttack / 10));
                _noMove |= indicator.Card.BCards.Any(s =>
                    s.Type == (byte)CardType.Move
                    && s.SubType.Equals((byte)AdditionalTypes.Move.MovementImpossible / 10));
            }
        }

        public void GenerateMateTransportId()
        {
            int nextId = ServerManager.Instance.MateIds.Count > 0 ? ServerManager.Instance.MateIds.Last() + 1 : 2000000;
            ServerManager.Instance.MateIds.Add(nextId);
            MateTransportId = nextId;
        }

        public string GenerateCMode(short morphId) => $"c_mode 2 {MateTransportId} {morphId} 0 0";

        public string GenerateCond() => $"cond 2 {MateTransportId} 0 0 {Speed}";

        public string GenerateEInfo() =>
            $"e_info 10 {NpcMonsterVNum} {Level} {Monster.Element} {Monster.AttackClass} {Monster.ElementRate} {Monster.AttackUpgrade} {Monster.DamageMinimum} {Monster.DamageMaximum} {Monster.Concentrate} {Monster.CriticalChance} {Monster.CriticalRate} {Monster.DefenceUpgrade} {Monster.CloseDefence} {Monster.DefenceDodge} {Monster.DistanceDefence} {Monster.DistanceDefenceDodge} {Monster.MagicDefence} {Monster.FireResistance} {Monster.WaterResistance} {Monster.LightResistance} {Monster.DarkResistance} {Monster.MaxHP} {Monster.MaxMP} -1 {Name.Replace(' ', '^')}";

        public string GenerateIn(bool foe = false, bool isAct4 = false)
        {
            if (_owner.Invisible || _owner.InvisibleGm)
            {
            return ""; //Maybe have to implement the exception on each mate.GenerateIn call.
             }
            string name = Name.Replace(' ', '^');
            if (foe)
            {
                name = "!§$%&/()=?*+~#";
            }

            int faction = 0;
            if (isAct4)
            {
                faction = (byte)Owner.Faction + 2;
            }

            return
                $"in 2 {NpcMonsterVNum} {MateTransportId} {PositionX} {PositionY} {Direction} {(int)(Hp / (float)MaxHp * 100)} {(int)(Mp / (float)MaxMp * 100)} 0 {faction} 3 {CharacterId} 1 0 {(IsUsingSp && SpInstance != null ? SpInstance.Item.Morph : (Skin != 0 ? Skin : -1))} {name} 0 -1 0 0 0 0 0 0 0 0";
        }

        public string GenerateOut() => $"out 2 {MateTransportId}";

        public string GenerateRest()
        {
            IsSitting = !IsSitting;
            return $"rest 2 {MateTransportId} {(IsSitting ? 1 : 0)}";
        }

        public string GenerateScPacket()
        {
            double xp = XpLoad();
            if (xp > int.MaxValue)
            {
                xp = (int)(xp / 100);
            }

            switch (MateType)
            {
                case MateType.Partner:
                    return
                        $"sc_n {PetId} {NpcMonsterVNum} {MateTransportId} {Level} {Loyalty} {Experience} {(WeaponInstance != null ? $"{WeaponInstance.ItemVNum}.{WeaponInstance.Rare}.{WeaponInstance.Upgrade}" : "-1")} {(ArmorInstance != null ? $"{ArmorInstance.ItemVNum}.{ArmorInstance.Rare}.{ArmorInstance.Upgrade}" : "-1")} {(GlovesInstance != null ? $"{GlovesInstance.ItemVNum}.0.0" : "-1")} {(BootsInstance != null ? $"{BootsInstance.ItemVNum}.0.0" : "-1")} 0 0 1 {WeaponInstance?.Upgrade ?? 0} {Monster.DamageMinimum + BaseDamage + (WeaponInstance?.DamageMinimum ?? 0)} {Monster.DamageMaximum + BaseDamage + (WeaponInstance?.DamageMaximum ?? 0)} {Monster.Concentrate + (WeaponInstance?.HitRate ?? 0)} {Monster.CriticalChance + (WeaponInstance?.CriticalLuckRate ?? 0)} {Monster.CriticalRate + (WeaponInstance?.CriticalRate ?? 0)} {ArmorInstance?.Upgrade ?? 0} {Monster.CloseDefence + MeleeDefense + (ArmorInstance?.CloseDefence ?? 0) + (GlovesInstance?.CloseDefence ?? 0) + (BootsInstance?.CloseDefence ?? 0)} {Monster.DefenceDodge + MeleeDefenseDodge + (ArmorInstance?.DefenceDodge ?? 0) + (GlovesInstance?.DefenceDodge ?? 0) + (BootsInstance?.DefenceDodge ?? 0)} {Monster.DistanceDefence + RangeDefense + (ArmorInstance?.DistanceDefence ?? 0) + (GlovesInstance?.DistanceDefence ?? 0) + (BootsInstance?.DistanceDefence ?? 0)} {Monster.DistanceDefenceDodge + RangeDefenseDodge + (ArmorInstance?.DistanceDefenceDodge ?? 0) + (GlovesInstance?.DistanceDefenceDodge ?? 0) + (BootsInstance?.DistanceDefenceDodge ?? 0)} {Monster.MagicDefence + MagicalDefense + (ArmorInstance?.MagicDefence ?? 0) + (GlovesInstance?.MagicDefence ?? 0) + (BootsInstance?.MagicDefence ?? 0)} {0 /*SP Element*/} {Monster.FireResistance + (GlovesInstance?.FireResistance ?? 0) + (BootsInstance?.FireResistance ?? 0)} {Monster.WaterResistance + (GlovesInstance?.WaterResistance ?? 0) + (BootsInstance?.WaterResistance ?? 0)} {Monster.LightResistance + (GlovesInstance?.LightResistance ?? 0) + (BootsInstance?.LightResistance ?? 0)} {Monster.DarkResistance + (GlovesInstance?.DarkResistance ?? 0) + (BootsInstance?.DarkResistance ?? 0)} {Hp} {MaxHp} {Mp} {MaxMp} 0 {xp} {(IsUsingSp ? SpInstance.Item.Name.Replace(' ', '^') : Name.Replace(' ', '^'))} {(IsUsingSp && SpInstance != null ? SpInstance.Item.Morph : Skin != 0 ? Skin : -1)} {(IsSummonable ? 1 : 0)} {(SpInstance != null ? $"{SpInstance.ItemVNum}.100" : "-1")} {(SpInstance != null ? "0.0 0.0 0.0" : "-1 -1 -1")}";

                case MateType.Pet:
                    return
                        $"sc_p {PetId} {NpcMonsterVNum} {MateTransportId} {Level} {Loyalty} {Experience} 0 {Monster.AttackUpgrade + Attack} {Monster.DamageMinimum + BaseDamage} {Monster.DamageMaximum + BaseDamage} {Monster.Concentrate} {Monster.CriticalChance} {Monster.CriticalRate} {Monster.DefenceUpgrade + Defence} {Monster.CloseDefence + MeleeDefense} {Monster.DefenceDodge + MeleeDefenseDodge} {Monster.DistanceDefence + RangeDefense} {Monster.DistanceDefenceDodge + RangeDefenseDodge} {Monster.MagicDefence + MagicalDefense} {Monster.Element} {Monster.FireResistance} {Monster.WaterResistance} {Monster.LightResistance} {Monster.DarkResistance} {Hp} {MaxHp} {Mp} {MaxMp} 0 {xp} {(CanPickUp ? 1 : 0)} {Name.Replace(' ', '^')} {(IsSummonable ? 1 : 0)}";
            }

            return string.Empty;
        }

        public string GenerateStatInfo() =>
            $"st 2 {MateTransportId} {Level} 0 {(int)((float)Hp / (float)MaxHp * 100)} {(int)((float)Mp / (float)MaxMp * 100)} {Hp} {Mp}{Buff.GetAllItems().Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}";

        public void GenerateXp(int xp)
        {
            if (Level < ServerManager.Instance.Configuration.MaxLevel)
            {
                Experience += xp;
                if (Experience >= XpLoad())
                {
                    if (Level + 1 < Owner.Level)
                    {
                        Experience = (long)(Experience - XpLoad());
                        Level++;
                        Hp = MaxHp;
                        Mp = MaxMp;
                        Owner.MapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, MateTransportId, 4732),
                            PositionX, PositionY);
                        Owner.MapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, MateTransportId, 4798),
                            PositionX, PositionY);
                    }
                }
            }

            Owner.Session.SendPacket(GenerateScPacket());
        }

        public string GenPski()
        {
            string skills = string.Empty;
            if (Skills != null)
            {
                foreach (Skill s in Skills)
                {
                    skills += $" {s.SkillVNum}";
                }
            }

            return $"pski{skills}";
        }

        public int[] GetBuff(CardType type, byte subtype)
        {
            int value1 = 0;
            int value2 = 0;

            foreach (Buff buff in Buff.Where(s => s?.Card?.BCards != null))
            {
                foreach (BCard entry in buff.Card.BCards.Where(s =>
                    s.Type.Equals((byte)type) && s.SubType.Equals(subtype)
                                               && (s.CastType != 1
                                                   || (s.CastType == 1 &&
                                                       buff.Start.AddMilliseconds(buff.Card.Delay * 100) <
                                                       DateTime.Now))))
                {
                    if (entry.IsLevelScaled)
                    {
                        if (entry.IsLevelDivided)
                        {
                            value1 += buff.Level / entry.FirstData;
                        }
                        else
                        {
                            value1 += entry.FirstData * buff.Level;
                        }
                    }
                    else
                    {
                        value1 += entry.FirstData;
                    }

                    value2 += entry.SecondData;
                }
            }

            return new[] { value1, value2 };
        }

        public List<ItemInstance> GetInventory()
        {
            switch (PetId)
            {
                case 0:
                    return Owner.Inventory.Where(s => s.Type == InventoryType.FirstPartnerInventory);

                case 1:
                    return Owner.Inventory.Where(s => s.Type == InventoryType.SecondPartnerInventory);

                case 2:
                    return Owner.Inventory.Where(s => s.Type == InventoryType.ThirdPartnerInventory);
            }

            return new List<ItemInstance>();
        }

        public int HpLoad()
        {
            double multiplicator = 1.0;
            int hp = 0;

            multiplicator += GetBuff(CardType.BearSpirit, (byte)AdditionalTypes.BearSpirit.IncreaseMaximumHP)[0]
                             / 100D;
            multiplicator += GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.IncreasesMaximumHP)[0] / 100D;
            hp += GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPIncreased)[0];
            hp -= GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPDecreased)[0];
            hp += GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPMPIncreased)[0];

            // Monster Bonus HP
            hp += (int)(Monster.MaxHP - MateHelper.Instance.HpData[Monster.Level]);

            return (int)((MateHelper.Instance.HpData[Level] + hp) * multiplicator);
        }

        public int BaseDamageLoad()
        {
            return MateHelper.Instance.DamageData[GetMateType(), Level > 0 ? Level - 1 : 0];
        }
        public int MeleeDefenseLoad()
        {
            return MateHelper.Instance.MeleeDefenseData[GetMateType(), Level > 0 ? Level - 1 : 0];
        }
        public int MeleeDefenseDodgeLoad()
        {
            return MateHelper.Instance.MeleeDefenseDodgeData[GetMateType(), Level > 0 ? Level - 1 : 0];
        }
        public int RangeDefenseLoad()
        {
            return MateHelper.Instance.RangeDefenseData[GetMateType(), Level > 0 ? Level - 1 : 0];
        }
        public int RangeDefenseDodgeLoad()
        {
            return MateHelper.Instance.RangeDefenseDodgeData[GetMateType(), Level > 0 ? Level - 1 : 0];
        }
        public int MagicalDefenseLoad()
        {
            return MateHelper.Instance.MagicDefenseData[GetMateType(), Level > 0 ? Level - 1 : 0];
        }

        public string GenerateRc(int characterHealth) => $"rc 2 {MateTransportId} {characterHealth} 0";

        /// <summary>
        /// Checks if the current character is in range of the given position
        /// </summary>
        /// <param name="xCoordinate">The x coordinate of the object to check.</param>
        /// <param name="yCoordinate">The y coordinate of the object to check.</param>
        /// <param name="range">The range of the coordinates to be maximal distanced.</param>
        /// <returns>True if the object is in Range, False if not.</returns>
        public bool IsInRange(int xCoordinate, int yCoordinate, int range) =>
            Math.Abs(PositionX - xCoordinate) <= range && Math.Abs(PositionY - yCoordinate) <= range;

        public void LoadInventory()
        {
            List<ItemInstance> inv = GetInventory();
            if (inv.Count == 0)
            {
                return;
            }

            WeaponInstance = inv.Find(s => s.Item.EquipmentSlot == EquipmentType.MainWeapon);
            ArmorInstance = inv.Find(s => s.Item.EquipmentSlot == EquipmentType.Armor);
            GlovesInstance = inv.Find(s => s.Item.EquipmentSlot == EquipmentType.Gloves);
            BootsInstance = inv.Find(s => s.Item.EquipmentSlot == EquipmentType.Boots);
            SpInstance = inv.Find(s => s.Item.EquipmentSlot == EquipmentType.Sp);
        }

        public int MpLoad()
        {
            int mp = 0;
            double multiplicator = 1.0;
            multiplicator += GetBuff(CardType.BearSpirit, (byte)AdditionalTypes.BearSpirit.IncreaseMaximumMP)[0]
                             / 100D;
            multiplicator += GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.IncreasesMaximumMP)[0] / 100D;
            mp += GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumMPIncreased)[0];
            mp -= GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPDecreased)[0];
            mp += GetBuff(CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPMPIncreased)[0];

            // Monster Bonus MP
            mp += (int)(Monster.MaxMP - (Monster.Race == 0
                             ? MateHelper.Instance.PrimaryMpData[Monster.Level]
                             : MateHelper.Instance.SecondaryMpData[Monster.Level]));

            return (int)(((Monster.Race == 0
                               ? MateHelper.Instance.PrimaryMpData[Level]
                               : MateHelper.Instance.SecondaryMpData[Level]) + mp) * multiplicator);
        }

        private void RemoveBuff(short id)
        {
            Buff indicator = Buff[id];

            if (indicator != null)
            {
                Buff.Remove(id);
                _noAttack &= !indicator.Card.BCards.Any(s =>
                    s.Type == (byte)CardType.SpecialAttack
                    && s.SubType.Equals((byte)AdditionalTypes.SpecialAttack.NoAttack / 10));
                _noMove &= !indicator.Card.BCards.Any(s =>
                    s.Type == (byte)CardType.Move
                    && s.SubType.Equals((byte)AdditionalTypes.Move.MovementImpossible / 10));
            }
        }

        private double XpLoad()
        {
            try
            {
                return MateHelper.Instance.XpData[Level - 1];
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        public void UpdateBushFire()
        {
            BrushFireJagged = BestFirstSearch.LoadBrushFireJagged(new GridPos
            {
                X = PositionX,
                Y = PositionY
            }, Owner.Session.CurrentMapInstance.Map.JaggedGrid);
        }

        public void GetDamage(int damage)
        {
            Owner.LastDefence = DateTime.Now;
            Owner.Dispose();

            Hp -= damage;
            if (Hp < 0)
            {
                Hp = 0;
            }

        }

        private byte GetMateType()
        {
            byte b = 0;

            // Set b according to the desired Mate Type, Race or VNum

            return b;
        }

        internal void DisableBuffs(List<BuffType> buffsToDisable, int firstData)
        {
            throw new NotImplementedException();
        }
    }
}