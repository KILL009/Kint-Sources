using OpenNos.DAL.EF;
using OpenNos.Data;
using System;

namespace OpenNos.Mapper.Mappers
{
    public class StaticBonusMapper
    {
        public StaticBonusMapper()
        {

        }

        public bool ToStaticBonusDTO(StaticBonus input, StaticBonusDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.DateEnd = input.DateEnd;
            output.StaticBonusId = input.StaticBonusId;
            output.StaticBonusType = input.StaticBonusType;
            return true;
        }

        public bool ToStaticBonus(StaticBonusDTO input, StaticBonus output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.DateEnd = input.DateEnd;
            output.StaticBonusId = input.StaticBonusId;
            output.StaticBonusType = input.StaticBonusType;
            return true;
        }
    }
}
