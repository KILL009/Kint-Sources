using OpenNos.DAL.EF;
using OpenNos.Data;
using System;

namespace OpenNos.Mapper.Mappers
{
    public class MapDesignObjectMapper
    {
        public MapDesignObjectMapper()
        {

        }

        public bool ToMinilandObjectDTO(MinilandObject input, MinilandObjectDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.ItemInstanceId = input.ItemInstanceId;
            output.Level1BoxAmount = input.Level1BoxAmount;
            output.Level2BoxAmount = input.Level2BoxAmount;
            output.Level3BoxAmount = input.Level3BoxAmount;
            output.Level4BoxAmount = input.Level4BoxAmount;
            output.Level5BoxAmount = input.Level5BoxAmount;
            output.MapX = input.MapX;
            output.MapY = input.MapY;
            output.MinilandObjectId = input.MinilandObjectId;
            return true;
        }

        public bool ToMinilandObject(MinilandObjectDTO input, MinilandObject output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.ItemInstanceId = input.ItemInstanceId;
            output.Level1BoxAmount = input.Level1BoxAmount;
            output.Level2BoxAmount = input.Level2BoxAmount;
            output.Level3BoxAmount = input.Level3BoxAmount;
            output.Level4BoxAmount = input.Level4BoxAmount;
            output.Level5BoxAmount = input.Level5BoxAmount;
            output.MapX = input.MapX;
            output.MapY = input.MapY;
            output.MinilandObjectId = input.MinilandObjectId;
            return true;
        }
    }
}
