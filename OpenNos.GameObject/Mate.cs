using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class Mate : MateDTO
    {
        #region Members

        private NpcMonster monster;

        private Character owner;

        #endregion

        #region Instantiation

        public Mate()
        {
        }

        public Mate(Character owner, NpcMonster npcMonster, byte level, MateType matetype)
        {
            NpcMonsterVNum = npcMonster.NpcMonsterVNum;
            Monster = npcMonster;
            Hp = npcMonster.MaxHP;
            Mp = npcMonster.MaxMP;
            Name = npcMonster.Name;
            MateType = matetype;
            Level = level;
            Loyalty = 1000;
            PositionX = owner.PositionX;
            PositionY = owner.PositionY;
            MapX = PositionX;
            MapY = PositionY;
            Direction = owner.Direction;
            CharacterId = owner.CharacterId;
            Owner = owner;
            GeneateMateTransportId();
        }

        #endregion

        #region Properties

        public ItemInstance ArmorInstance { get; set; }

        public ItemInstance BootsInstance { get; set; }

        public ItemInstance GlovesInstance { get; set; }

        public bool IsSitting { get; set; }

        public bool IsUsingSp { get; set; }

        public int MateTransportId { get; set; }

        public int MaxHp => Monster.MaxHP;

        public int MaxMp => Monster.MaxMP;

        public NpcMonster Monster
        {
            get
            {
                return monster ?? ServerManager.Instance.GetNpc(NpcMonsterVNum);
            }
            set
            {
                monster = value;
            }
        }

        public Character Owner
        {
            get
            {
                return owner ?? ServerManager.Instance.GetSessionByCharacterId(CharacterId).Character;
            }
            set
            {
                owner = value;
            }
        }

        public byte PetId { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public ItemInstance SpInstance { get; set; }

        public ItemInstance WeaponInstance { get; set; }

        #endregion

        #region Methods

        public void GeneateMateTransportId()
        {
            int nextId = ServerManager.Instance.MateIds.Any() ? ServerManager.Instance.MateIds.Last() + 1 : 10000;
            ServerManager.Instance.MateIds.Add(nextId);
            MateTransportId = nextId;
        }

        public string GenerateCMode(short morphId) => $"c_mode 2 {MateTransportId} {morphId} 0 0";

        public string GenerateCond() => $"cond 2 {MateTransportId} 0 0 {Monster.Speed}";

        public EffectPacket GenerateEff(int effectid)
        {
            return new EffectPacket
            {
                EffectType = 2,
                CharacterId = MateTransportId,
                Id = effectid
            };
        }

        public string GenerateEInfo()
        {
            return $"e_info 10 {NpcMonsterVNum} {Level} {Monster.Element} {Monster.AttackClass} {Monster.ElementRate} {Monster.AttackUpgrade} {Monster.DamageMinimum} {Monster.DamageMaximum} {Monster.Concentrate} {Monster.CriticalChance} {Monster.CriticalRate} {Monster.DefenceUpgrade} {Monster.CloseDefence} {Monster.DefenceDodge} {Monster.DistanceDefence} {Monster.DistanceDefenceDodge} {Monster.MagicDefence} {Monster.FireResistance} {Monster.WaterResistance} {Monster.LightResistance} {Monster.DarkResistance} {Monster.MaxHP} {Monster.MaxMP} -1 {Name.Replace(' ', '^')}";
        }

        public string GenerateIn(bool foe = false, bool isAct4 = false)
        {
            var name = Name.Replace(' ', '^');
            if (foe)
            {
                name = "!§$%&/()=?*+~#";
            }

            var faction = 0;
            if (isAct4)
            {
                faction = (byte)Owner.Faction + 2;
            }

            if (MateType == MateType.Partner)
            {
                return $"in 2 {NpcMonsterVNum} {MateTransportId} {(IsTeamMember ? PositionX : MapX)} {(IsTeamMember ? PositionY : MapY)} {Direction} {(int)(Hp / (float)MaxHp * 100)} {(int)(Mp / (float)MaxMp * 100)} 0 {faction} 3 {CharacterId} 1 0 {(IsUsingSp && SpInstance != null ? SpInstance.Item.Morph : (Skin != 0 ? Skin : -1))} {name} 0 -1 0 0 0 0 0 0 0 0";
            }
            else
            {
                return $"in 2 {NpcMonsterVNum} {MateTransportId} {(IsTeamMember ? PositionX : MapX)} {(IsTeamMember ? PositionY : MapY)} {Direction} {(int)(Hp / (float)MaxHp * 100)} {(int)(Mp / (float)MaxMp * 100)} 0 {faction} 3 {CharacterId} 1 0 0 {name} 0 -1 0 0 0 0 0 0 0 0";
            }
        }

        public string GenerateOut() => $"out 2 {MateTransportId}";

        public string GenerateRest()
        {
            IsSitting = !IsSitting;
            return $"rest 2 {MateTransportId} {(IsSitting ? 1 : 0)}";
        }

        public string GenerateSay(string message, int type) => $"say 2 {MateTransportId} 2 {message}";

        public string GenerateScPacket()
        {
            switch (MateType)
            {
                case MateType.Partner:
                    return $"sc_n {PetId} {NpcMonsterVNum} {MateTransportId} {Level} {Loyalty} {GetRealEXP(Experience)} {(WeaponInstance != null ? $"{WeaponInstance.ItemVNum}.{WeaponInstance.Rare}.{WeaponInstance.Upgrade}" : "-1")} {(ArmorInstance != null ? $"{ArmorInstance.ItemVNum}.{ArmorInstance.Rare}.{ArmorInstance.Upgrade}" : "-1")} {(GlovesInstance != null ? $"{GlovesInstance.ItemVNum}.0.0" : "-1")} {(BootsInstance != null ? $"{BootsInstance.ItemVNum}.0.0" : "-1")} 0 0 1 0 142 174 232 4 70 0 73 158 86 158 69 0 0 0 0 0 2641 2641 1065 1065 0 285816 {(SpInstance != null ? "SP_NAME" : Name.Replace(' ', '^'))} {SpInstance?.Item.Morph ?? (Skin != 0 ? Skin : -1)} {(IsSummonable ? 1 : 0)} {(SpInstance != null ? $"{SpInstance.ItemVNum}.100" : "-1")} -1 -1 -1";

                case MateType.Pet:
                    return $"sc_p {PetId} {NpcMonsterVNum} {MateTransportId} {Level} {Loyalty} {GetRealEXP(Experience)} 0 {Monster.AttackUpgrade} {Monster.DamageMinimum} {Monster.DamageMaximum} {Monster.Concentrate} {Monster.CriticalChance} {Monster.CriticalRate} {Monster.DefenceUpgrade} {Monster.CloseDefence} {Monster.DefenceDodge} {Monster.DistanceDefence} {Monster.DistanceDefenceDodge} {Monster.MagicDefence} {Monster.Element} {Monster.FireResistance} {Monster.WaterResistance} {Monster.LightResistance} {Monster.DarkResistance} {Hp} {MaxHp} {Mp} {MaxMp} 0 15 {(CanPickUp ? 1 : 0)} {Name.Replace(' ', '^')} {(IsSummonable ? 1 : 0)}";
            }

            return string.Empty;
        }

        public string GenerateStatInfo()
        {
            return $"st 2 {MateTransportId} {Level} 0 {(int)((float)Hp / (float)MaxHp * 100)} {(int)((float)Mp / (float)MaxMp * 100)} {Hp} {Mp}";
        }

        public void GenerateXp(int xp)
        {
            if (Level < ServerManager.Instance.MaxLevel)
            {
                Experience += xp;
                if (Experience >= XpLoad())
                {
                    if (Level == 98)
                        Experience = 0;
                    if (Level + 1 < Owner.Level)
                    {
                        Experience = (long)(Experience - XpLoad());
                        Level++;

                        Hp = MaxHp;
                        Mp = MaxMp;
                        Owner.MapInstance?.Broadcast(GenerateEff(6), PositionX, PositionY);
                        Owner.MapInstance?.Broadcast(GenerateEff(198), PositionX, PositionY);
                    }
                }
            }

            ServerManager.Instance.GetSessionByCharacterId(Owner.CharacterId).SendPacket(GenerateScPacket());
        }

        public long GetRealEXP(long exp)
        {
            var final = 0;

            switch (MateType)
            {
                case MateType.Pet:
                    {
                        var Percent = (int)((float)(exp / XpLoad()) * 100);

                        if (Percent < 6)
                            final = 0;
                        else if (Percent >= 6 && Percent < 13)
                            final = 1;
                        else if (Percent >= 13 && Percent < 20)
                            final = 2;
                        else if (Percent >= 20 && Percent < 26)
                            final = 3;
                        else if (Percent >= 26 && Percent < 33)
                            final = 4;
                        else if (Percent >= 33 && Percent < 40)
                            final = 5;
                        else if (Percent >= 40 && Percent < 46)
                            final = 6;
                        else if (Percent >= 46 && Percent < 53)
                            final = 7;
                        else if (Percent >= 53 && Percent < 60)
                            final = 8;
                        else if (Percent >= 60 && Percent < 66)
                            final = 9;
                        else if (Percent >= 66 && Percent < 73)
                            final = 10;
                        else if (Percent >= 73 && Percent < 80)
                            final = 11;
                        else if (Percent >= 80 && Percent < 86)
                            final = 12;
                        else if (Percent >= 86 && Percent < 93)
                            final = 13;
                        else if (Percent >= 93 && Percent < 100)
                            final = 14;
                        else
                            final = 15;

                        return final;
                    }
                case MateType.Partner:
                    return exp;
            }

            return 0;
        }

        public override void Initialize()
        {
        }

        /// <summary>
        /// Checks if the current character is in range of the given position
        /// </summary>
        /// <param name="xCoordinate">The x coordinate of the object to check.</param>
        /// <param name="yCoordinate">The y coordinate of the object to check.</param>
        /// <param name="range">The range of the coordinates to be maximal distanced.</param>
        /// <returns>True if the object is in Range, False if not.</returns>
        public bool IsInRange(int xCoordinate, int yCoordinate, int range)
        {
            return Math.Abs(PositionX - xCoordinate) <= range && Math.Abs(PositionY - yCoordinate) <= range;
        }

        public void LoadInventory()
        {
            List<ItemInstance> inv = GetInventory();
            if (inv.Count == 0)
            {
                return;
            }

            WeaponInstance = inv.FirstOrDefault(s => s.Item.EquipmentSlot == EquipmentType.MainWeapon);
            ArmorInstance = inv.FirstOrDefault(s => s.Item.EquipmentSlot == EquipmentType.Armor);
            GlovesInstance = inv.FirstOrDefault(s => s.Item.EquipmentSlot == EquipmentType.Gloves);
            BootsInstance = inv.FirstOrDefault(s => s.Item.EquipmentSlot == EquipmentType.Boots);
            SpInstance = inv.FirstOrDefault(s => s.Item.EquipmentSlot == EquipmentType.Sp);
        }

        private List<ItemInstance> GetInventory()
        {
            List<ItemInstance> items = new List<ItemInstance>();
            switch (PetId)
            {
                case 0:
                    items = Owner.Inventory.Select(s => s.Value).Where(s => s.Type == InventoryType.FirstPartnerInventory).ToList();
                    break;

                case 1:
                    items = Owner.Inventory.Select(s => s.Value).Where(s => s.Type == InventoryType.SecondPartnerInventory).ToList();
                    break;

                case 2:
                    items = Owner.Inventory.Select(s => s.Value).Where(s => s.Type == InventoryType.ThirdPartnerInventory).ToList();
                    break;
            }

            return items;
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
    }
}