using OpenNos.Data;
using OpenNos.Domain;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class BattleEntity
    {
        #region Instantiation

        public BattleEntity(Character character, Skill skill)
        {
            Session = character.Session;
            HpMax = character.HPMax;
            MpMax = character.MPMax;
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
                        if (character.Class == ClassType.Adventurer || character.Class == ClassType.Swordman || character.Class == ClassType.Magician)
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

                ShellWeaponEffects = character.ShellEffectMain.ToList();
            }

            ItemInstance armor = character.Inventory.LoadBySlotAndType((byte)EquipmentType.Armor, InventoryType.Wear);
            if (armor != null)
            {
                DefenseUpgrade = armor.Upgrade;
                ArmorMeleeDefense = armor.CloseDefence + armor.Item.CloseDefence;
                ArmorRangeDefense = armor.DistanceDefence + armor.Item.DistanceDefence;
                ArmorMagicalDefense = armor.MagicDefence + armor.Item.MagicDefence;

                ShellArmorEffects = character.ShellEffectArmor.ToList();
            }

            CellonOptions = Session.Character.CellonOptions.GetAllItems();

            MeleeDefense = character.Defence - ArmorMeleeDefense;
            MeleeDefenseDodge = character.DefenceRate;
            RangeDefense = character.DistanceDefence - ArmorRangeDefense;
            RangeDefenseDodge = character.DistanceDefenceRate;
            MagicalDefense = character.MagicalDefence - ArmorMagicalDefense;
            Element = character.Element;
            ElementRate = character.ElementRate + character.ElementRateSP;
        }

        public BattleEntity(Mate mate)
        {
            HpMax = mate.MaxHp;
            MpMax = mate.MaxMp;

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

        public BattleEntity(MapMonster monster)
        {
            HpMax = monster.Monster.MaxHP;
            MpMax = monster.Monster.MaxMP;
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
            HpMax = npc.Npc.MaxHP;
            MpMax = npc.Npc.MaxMP;

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

        #endregion

        #region Properties

        public int ArmorDefense { get; set; }

        public int ArmorMagicalDefense { get; }

        public int ArmorMeleeDefense { get; }

        public int ArmorRangeDefense { get; }

        public AttackType AttackType { get; }

        public short AttackUpgrade { get; set; }

        public List<BCard> BCards { get; }

        public List<Buff> Buffs { get; }

        public List<CellonOptionDTO> CellonOptions { get; }

        public int CritChance { get; set; }

        public int CritRate { get; set; }

        public int DamageMaximum { get; }

        public int DamageMinimum { get; }

        public int Defense { get; set; }

        public short DefenseUpgrade { get; set; }

        public int Dodge { get; set; }

        public byte Element { get; }

        public int ElementRate { get; }

        public EntityType EntityType { get; }

        public int FireResistance { get; set; }

        public int Hitrate { get; }

        public int HpMax { get; }

        public bool Invincible { get; set; }

        public int Level { get; set; }

        public int LightResistance { get; set; }

        public int MagicalDefense { get; }

        public int MeleeDefense { get; }

        public int MeleeDefenseDodge { get; }

        public int Morale { get; set; }

        public int MpMax { get; set; }

        public short PositionX { get; }

        public short PositionY { get; }

        public int RangeDefense { get; }

        public int RangeDefenseDodge { get; }

        public int Resistance { get; set; }

        public ClientSession Session { get; }

        public int ShadowResistance { get; set; }

        public List<ShellEffectDTO> ShellArmorEffects { get; }

        public List<ShellEffectDTO> ShellWeaponEffects { get; }

        public int WaterResistance { get; set; }

        public int WeaponDamageMaximum { get; }

        public int WeaponDamageMinimum { get; }
        public static IEnumerable<object> StaticBcards { get; internal set; }

        #endregion
    }
}