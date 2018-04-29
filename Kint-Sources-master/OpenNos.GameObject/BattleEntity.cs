using OpenNos.Data;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;


namespace OpenNos.GameObject.Battle
{
    public class BattleEntity
    {
        private Character character;
        #region Instantiation

        public BattleEntity(Character character, Skill skill)
        {
            Session = character.Session;
            HPMax = character.HPMax;
            MPMax = character.MPMax;
            Buffs = character.Buff.GetAllItems();
            BCards = character.EquipmentBCards.GetAllItems();
            Level = character.Level;
            EntityType = EntityType.Player;
            DamageMinimum = character.MinHit;
            DamageMaximum = character.MaxHit;
            Hitrate = character.HitRate;
            CritChance = character.HitCriticalRate;
            CritRate = character.HitCritical;
            Morale = character.Level;
            FireResistance = character.FireResistance;
            WaterResistance = character.WaterResistance;
            LightResistance = character.LightResistance;
            ShadowResistance = character.DarkResistance;
            PositionX = character.PositionX;
            PositionY = character.PositionY;

            ItemInstance weapon = null;

            if (skill != null)
            {
                switch (skill.Type)
                {
                    case 0:
                        AttackType = AttackType.Melee;
                        if (character.Class == ClassType.Archer)
                        {
                            DamageMinimum = character.MinDistance;
                            DamageMaximum = character.MaxDistance;
                            Hitrate = character.DistanceRate;
                            CritChance = character.DistanceCriticalRate;
                            CritRate = character.DistanceCritical;
                            weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
                        }
                        else
                        {
                            weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                        }
                        break;

                    case 1:
                        AttackType = AttackType.Range;
                        if (character.Class == ClassType.Adventurer || character.Class == ClassType.Swordman || character.Class == ClassType.Magician || character.Class == ClassType.Fighter)
                        {
                            DamageMinimum = character.MinDistance;
                            DamageMaximum = character.MaxDistance;
                            Hitrate = character.DistanceRate;
                            CritChance = character.DistanceCriticalRate;
                            CritRate = character.DistanceCritical;
                            weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
                        }
                        else
                        {
                            weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                        }
                        break;

                    case 2:
                        AttackType = AttackType.Magical;
                        weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                        break;

                    case 3:
                        weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                        switch (character.Class)
                        {
                            case ClassType.Adventurer:
                            case ClassType.Fighter:
                            case ClassType.Swordman:
                                AttackType = AttackType.Melee;
                                break;

                            case ClassType.Archer:
                                AttackType = AttackType.Range;
                                break;

                            case ClassType.Magician:
                                AttackType = AttackType.Magical;
                                break;
                        }
                        break;

                    case 5:
                        AttackType = AttackType.Melee;
                        switch (character.Class)
                        {
                            case ClassType.Adventurer:
                            case ClassType.Swordman:
                            case ClassType.Fighter:
                            case ClassType.Magician:
                                weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                                break;

                            case ClassType.Archer:
                                weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
                                break;
                        }
                        break;
                }
            }
            else
            {
                weapon = character.Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon, InventoryType.Wear);
                switch (character.Class)
                {
                    case ClassType.Adventurer:
                    case ClassType.Fighter:
                    case ClassType.Swordman:
                        AttackType = AttackType.Melee;
                        break;

                    case ClassType.Archer:
                        AttackType = AttackType.Range;
                        break;

                    case ClassType.Magician:
                        AttackType = AttackType.Magical;
                        break;
                }
            }

            if (weapon != null)
            {
                AttackUpgrade = weapon.Upgrade;
                WeaponDamageMinimum = weapon.DamageMinimum + weapon.Item.DamageMinimum;
                WeaponDamageMaximum = weapon.DamageMaximum + weapon.Item.DamageMinimum;

                ShellWeaponEffects = new List<ShellEffectDTO>(weapon.ShellEffects);
            }

            ItemInstance armor = character.Inventory.LoadBySlotAndType((byte)EquipmentType.Armor, InventoryType.Wear);
            if (armor != null)
            {
                DefenseUpgrade = armor.Upgrade;
                ArmorMeleeDefense = armor.CloseDefence + armor.Item.CloseDefence;
                ArmorRangeDefense = armor.DistanceDefence + armor.Item.DistanceDefence;
                ArmorMagicalDefense = armor.MagicDefence + armor.Item.MagicDefence;

                ShellArmorEffects = new List<ShellEffectDTO>(armor.ShellEffects);
            }

            CellonOptions = Session.Character.CellonOptions.GetAllItems();

            MeleeDefense = character.Defence;
            MeleeDefenseDodge = character.DefenceRate;
            RangeDefense = character.DistanceDefence;
            RangeDefenseDodge = character.DistanceDefenceRate;
            MagicalDefense = character.MagicalDefence;
            Element = character.Element;
            ElementRate = character.ElementRate + character.ElementRateSP;
        }

        public BattleEntity(Mate mate)
        {
            HPMax = mate.MaxHp;
            MPMax = mate.MaxMp;

            Buffs = mate.Buff.GetAllItems();
            BCards = mate.Monster.BCards.ToList();
            Level = mate.Level;
            EntityType = EntityType.Mate;
            DamageMinimum = (mate.WeaponInstance?.DamageMinimum ?? 0) + mate.BaseDamage + mate.Monster.DamageMinimum;
            DamageMaximum = (mate.WeaponInstance?.DamageMaximum ?? 0) + mate.BaseDamage + mate.Monster.DamageMaximum;
            WeaponDamageMinimum = (mate.WeaponInstance?.DamageMinimum ?? DamageMinimum);
            WeaponDamageMaximum = (mate.WeaponInstance?.DamageMaximum ?? DamageMaximum);
            Hitrate = mate.Monster.Concentrate + (mate.WeaponInstance?.HitRate ?? 0);
            CritChance = mate.Monster.CriticalChance + (mate.WeaponInstance?.CriticalLuckRate ?? 0);
            CritRate = mate.Monster.CriticalRate + (mate.WeaponInstance?.CriticalRate ?? 0);
            Morale = mate.Level;
            AttackUpgrade = mate.WeaponInstance?.Upgrade ?? mate.Attack;
            FireResistance = mate.Monster.FireResistance + (mate.GlovesInstance?.FireResistance ?? 0) + (mate.BootsInstance?.FireResistance ?? 0);
            WaterResistance = mate.Monster.WaterResistance + (mate.GlovesInstance?.FireResistance ?? 0) + (mate.BootsInstance?.FireResistance ?? 0);
            LightResistance = mate.Monster.LightResistance + (mate.GlovesInstance?.FireResistance ?? 0) + (mate.BootsInstance?.FireResistance ?? 0);
            ShadowResistance = mate.Monster.DarkResistance + (mate.GlovesInstance?.FireResistance ?? 0) + (mate.BootsInstance?.FireResistance ?? 0);
            PositionX = mate.PositionX;
            PositionY = mate.PositionY;
            AttackType = (AttackType)mate.Monster.AttackClass;

            DefenseUpgrade = mate.ArmorInstance?.Upgrade ?? mate.Defence;
            MeleeDefense = (mate.ArmorInstance?.CloseDefence ?? 0) + mate.MeleeDefense + mate.Monster.CloseDefence;
            RangeDefense = (mate.ArmorInstance?.DistanceDefence ?? 0) + mate.RangeDefense + mate.Monster.DistanceDefence;
            MagicalDefense = (mate.ArmorInstance?.MagicDefence ?? 0) + mate.MagicalDefense + mate.Monster.MagicDefence;
            MeleeDefenseDodge = (mate.ArmorInstance?.DefenceDodge ?? 0) + mate.MeleeDefenseDodge + mate.Monster.DefenceDodge;
            RangeDefenseDodge = (mate.ArmorInstance?.DistanceDefenceDodge ?? 0) + mate.RangeDefenseDodge + mate.Monster.DistanceDefenceDodge;

            ArmorMeleeDefense = mate.ArmorInstance?.CloseDefence ?? MeleeDefense;
            ArmorRangeDefense = mate.ArmorInstance?.DistanceDefence ?? RangeDefense;
            ArmorMagicalDefense = mate.ArmorInstance?.MagicDefence ?? MagicalDefense;

            Element = mate.Monster.Element;
            ElementRate = mate.Monster.ElementRate;
        }

        internal static void DisableBuffs(List<BuffType> types, int level)
        {
            throw new NotImplementedException();
        }

        public BattleEntity(MapMonster monster)
        {
            HPMax = monster.Monster.MaxHP;
            MPMax = monster.Monster.MaxMP;
            Buffs = monster.Buff.GetAllItems();
            BCards = monster.Monster.BCards.ToList();
            Level = monster.Monster.Level;
            EntityType = EntityType.Monster;
            DamageMinimum = 0;
            DamageMaximum = 0;
            WeaponDamageMinimum = monster.Monster.DamageMinimum;
            WeaponDamageMaximum = monster.Monster.DamageMaximum;
            Hitrate = monster.Monster.Concentrate;
            CritChance = monster.Monster.CriticalChance;
            CritRate = monster.Monster.CriticalRate;
            Morale = monster.Monster.Level;
            AttackUpgrade = monster.Monster.AttackUpgrade;
            FireResistance = monster.Monster.FireResistance;
            WaterResistance = monster.Monster.WaterResistance;
            LightResistance = monster.Monster.LightResistance;
            ShadowResistance = monster.Monster.DarkResistance;
            PositionX = monster.MapX;
            PositionY = monster.MapY;
            AttackType = (AttackType)monster.Monster.AttackClass;
            DefenseUpgrade = monster.Monster.DefenceUpgrade;
            MeleeDefense = monster.Monster.CloseDefence;
            MeleeDefenseDodge = monster.Monster.DefenceDodge;
            RangeDefense = monster.Monster.DistanceDefence;
            RangeDefenseDodge = monster.Monster.DistanceDefenceDodge;
            MagicalDefense = monster.Monster.MagicDefence;
            ArmorMeleeDefense = monster.Monster.CloseDefence;
            ArmorRangeDefense = monster.Monster.DistanceDefence;
            ArmorMagicalDefense = monster.Monster.MagicDefence;
            Element = monster.Monster.Element;
            ElementRate = monster.Monster.ElementRate;
        }

        public BattleEntity(MapNpc npc)
        {
            HPMax = npc.Npc.MaxHP;
            MPMax = npc.Npc.MaxMP;

            //npc.Buff.CopyTo(Buffs);
            BCards = npc.Npc.BCards.ToList();
            Level = npc.Npc.Level;
            EntityType = EntityType.Monster;
            DamageMinimum = 0;
            DamageMaximum = 0;
            WeaponDamageMinimum = npc.Npc.DamageMinimum;
            WeaponDamageMaximum = npc.Npc.DamageMaximum;
            Hitrate = npc.Npc.Concentrate;
            CritChance = npc.Npc.CriticalChance;
            CritRate = npc.Npc.CriticalRate;
            Morale = npc.Npc.Level;
            AttackUpgrade = npc.Npc.AttackUpgrade;
            FireResistance = npc.Npc.FireResistance;
            WaterResistance = npc.Npc.WaterResistance;
            LightResistance = npc.Npc.LightResistance;
            ShadowResistance = npc.Npc.DarkResistance;
            PositionX = npc.MapX;
            PositionY = npc.MapY;
            AttackType = (AttackType)npc.Npc.AttackClass;
            DefenseUpgrade = npc.Npc.DefenceUpgrade;
            MeleeDefense = npc.Npc.CloseDefence;
            MeleeDefenseDodge = npc.Npc.DefenceDodge;
            RangeDefense = npc.Npc.DistanceDefence;
            RangeDefenseDodge = npc.Npc.DistanceDefenceDodge;
            MagicalDefense = npc.Npc.MagicDefence;
            ArmorMeleeDefense = npc.Npc.CloseDefence;
            ArmorRangeDefense = npc.Npc.DistanceDefence;
            ArmorMagicalDefense = npc.Npc.MagicDefence;
            Element = npc.Npc.Element;
            ElementRate = npc.Npc.ElementRate;
        }

        public BattleEntity(Character character)
        {
            this.character = character;
        }

        #endregion

        #region Properties

        public int ArmorDefense { get; set; }

        public int ArmorMagicalDefense { get; set; }

        public int ArmorMeleeDefense { get; set; }

        public int ArmorRangeDefense { get; set; }

        public AttackType AttackType { get; set; }

        public short AttackUpgrade { get; set; }

        public List<BCard> BCards { get; set; }

        public List<Buff> Buffs { get; set; }

        public List<CellonOptionDTO> CellonOptions { get; set; }

        public int CritChance { get; set; }

        public int CritRate { get; set; }

        public int DamageMaximum { get; set; }

        public int DamageMinimum { get; set; }

        public int Defense { get; set; }

        public short DefenseUpgrade { get; set; }

        public int Dodge { get; set; }

        public byte Element { get; set; }

        public int ElementRate { get; set; }

        public EntityType EntityType { get; set; }

        public int FireResistance { get; set; }

        public int Hitrate { get; set; }

        public int HPMax { get; set; }

        public bool Invincible { get; set; }

        public byte Level { get; set; }

        public int LightResistance { get; set; }

        public int MagicalDefense { get; set; }

        public int MeleeDefense { get; set; }

        public int MeleeDefenseDodge { get; set; }

        public int Morale { get; set; }

        public int MPMax { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public int RangeDefense { get; set; }

        public int RangeDefenseDodge { get; set; }

        public int Resistance { get; set; }

        public ClientSession Session { get; set; }

        public int ShadowResistance { get; set; }

        public List<ShellEffectDTO> ShellArmorEffects { get; set; }

        public List<ShellEffectDTO> ShellWeaponEffects { get; set; }

        public int WaterResistance { get; set; }

        public int WeaponDamageMaximum { get; set; }

        public int WeaponDamageMinimum { get; set; }

        public List<BCard> StaticBcards { get; set; }

       
        #endregion
    }
}