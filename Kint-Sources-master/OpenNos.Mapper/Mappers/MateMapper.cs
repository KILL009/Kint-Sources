using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.Data;
using System;

namespace OpenNos.Mapper.Mappers
{
    public class MateMapper
    {
        public MateMapper()
        {

        }

        public bool ToMateDTO(Mate input, MateDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Attack = input.Attack;
            output.CanPickUp = input.CanPickUp;
            output.CharacterId = input.CharacterId;
            output.Defence = input.Defence;
            output.Direction = input.Direction;
            output.Experience = input.Experience;
            output.Hp = input.Hp;
            output.IsSummonable = input.IsSummonable;
            output.IsTeamMember = input.IsTeamMember;
            output.Level = input.Level;
            output.Loyalty = input.Loyalty;
            output.MapX = input.MapX;
            output.MapY = input.MapY;
            output.MateId = input.MateId;
            output.MateType = input.MateType;
            output.Mp = input.Mp;
            output.Name = input.Name;
            output.NpcMonsterVNum = input.NpcMonsterVNum;
            output.Skin = input.Skin;
            return true;
        }

        public bool ToMate(MateDTO input, Mate output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Attack = input.Attack;
            output.CanPickUp = input.CanPickUp;
            output.CharacterId = input.CharacterId;
            output.Defence = input.Defence;
            output.Direction = input.Direction;
            output.Experience = input.Experience;
            output.Hp = input.Hp;
            output.IsSummonable = input.IsSummonable;
            output.IsTeamMember = input.IsTeamMember;
            output.Level = input.Level;
            output.Loyalty = input.Loyalty;
            output.MapX = input.MapX;
            output.MapY = input.MapY;
            output.MateId = input.MateId;
            output.MateType = input.MateType;
            output.Mp = input.Mp;
            output.Name = input.Name;
            output.NpcMonsterVNum = input.NpcMonsterVNum;
            output.Skin = input.Skin;
            return true;
        }
    }
}
