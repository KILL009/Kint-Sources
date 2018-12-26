using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class NpcMonsterMapper
    {
        public NpcMonsterMapper()
        {
        }

        public bool ToNpcMonsterDTO(NpcMonster input, NpcMonsterDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.AmountRequired = input.AmountRequired;
            output.AttackClass = input.AttackClass;
            output.AttackUpgrade = input.AttackUpgrade;
            output.BasicArea = input.BasicArea;
            output.BasicCooldown = input.BasicCooldown;
            output.BasicRange = input.BasicRange;
            output.BasicSkill = input.BasicSkill;
            output.Catch = input.Catch;
            output.CloseDefence = input.CloseDefence;
            output.Concentrate = input.Concentrate;
            output.CriticalChance = input.CriticalChance;
            output.CriticalRate = input.CriticalRate;
            output.DamageMaximum = input.DamageMaximum;
            output.DamageMinimum = input.DamageMinimum;
            output.DarkResistance = (sbyte)input.DarkResistance;
            output.DefenceDodge = input.DefenceDodge;
            output.DefenceUpgrade = input.DefenceUpgrade;
            output.DistanceDefence = input.DistanceDefence;
            output.DistanceDefenceDodge = input.DistanceDefenceDodge;
            output.Element = input.Element;
            output.ElementRate = input.ElementRate;
            output.FireResistance = (sbyte)input.FireResistance;
            output.HeroLevel = input.HeroLevel;        
            output.HeroXp = input.HeroXP;           
            output.IsHostile = input.IsHostile;
            output.JobXP = input.JobXP;
            output.Level = input.Level;
            output.LightResistance = (sbyte)input.LightResistance;
            output.MagicDefence = input.MagicDefence;
            output.MaxHP = input.MaxHP;
            output.MaxMP = input.MaxMP;
            output.MonsterType = input.MonsterType;
            output.Name = input.Name;
            output.NoAggresiveIcon = input.NoAggresiveIcon;
            output.NoticeRange = input.NoticeRange;
            output.NpcMonsterVNum = input.NpcMonsterVNum;
            output.Race = input.Race;
            output.RaceType = input.RaceType;
            output.RespawnTime = input.RespawnTime;
            output.Speed = input.Speed;
            output.VNumRequired = input.VNumRequired;
            output.WaterResistance = (sbyte)input.WaterResistance;
            output.XP = input.XP;
            return true;
        }

        public bool ToNpcMonster(NpcMonsterDTO input, NpcMonster output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.AmountRequired = input.AmountRequired;
            output.AttackClass = input.AttackClass;
            output.AttackUpgrade = input.AttackUpgrade;
            output.BasicArea = input.BasicArea;
            output.BasicCooldown = input.BasicCooldown;
            output.BasicRange = input.BasicRange;
            output.BasicSkill = input.BasicSkill;
            output.Catch = input.Catch;
            output.CloseDefence = input.CloseDefence;
            output.Concentrate = input.Concentrate;
            output.CriticalChance = input.CriticalChance;
            output.CriticalRate = input.CriticalRate;
            output.DamageMaximum = input.DamageMaximum;
            output.DamageMinimum = input.DamageMinimum;
            output.DarkResistance = input.DarkResistance;
            output.DefenceDodge = input.DefenceDodge;
            output.DefenceUpgrade = input.DefenceUpgrade;
            output.DistanceDefence = input.DistanceDefence;
            output.DistanceDefenceDodge = input.DistanceDefenceDodge;
            output.Element = input.Element;
            output.ElementRate = input.ElementRate;
            output.FireResistance = input.FireResistance;
            output.HeroLevel = input.HeroLevel;           
            output.HeroXP = input.HeroXp;      
            output.IsHostile = input.IsHostile;
            output.JobXP = input.JobXP;
            output.Level = input.Level;
            output.LightResistance = input.LightResistance;
            output.MagicDefence = input.MagicDefence;
            output.MaxHP = input.MaxHP;
            output.MaxMP = input.MaxMP;
            output.MonsterType = input.MonsterType;
            output.Name = input.Name;
            output.NoAggresiveIcon = input.NoAggresiveIcon;
            output.NoticeRange = input.NoticeRange;
            output.NpcMonsterVNum = input.NpcMonsterVNum;
            output.Race = input.Race;
            output.RaceType = input.RaceType;
            output.RespawnTime = input.RespawnTime;
            output.Speed = input.Speed;
            output.VNumRequired = input.VNumRequired;
            output.WaterResistance = input.WaterResistance;
            output.XP = input.XP;
            return true;
        }
    }
}
