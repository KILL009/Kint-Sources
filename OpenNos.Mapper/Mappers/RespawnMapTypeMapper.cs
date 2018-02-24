using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class RespawnMapTypeMapper
    {
        public RespawnMapTypeMapper()
        {
        }

        public bool ToRespawnMapTypeDTO(RespawnMapType input, RespawnMapTypeDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.DefaultMapId = input.DefaultMapId;
            output.DefaultX = input.DefaultX;
            output.DefaultY = input.DefaultY;
            output.Name = input.Name;
            output.RespawnMapTypeId = input.RespawnMapTypeId;
            return true;
        }

        public bool ToRespawnMapType(RespawnMapTypeDTO input, RespawnMapType output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.DefaultMapId = input.DefaultMapId;
            output.DefaultX = input.DefaultX;
            output.DefaultY = input.DefaultY;
            output.Name = input.Name;
            output.RespawnMapTypeId = input.RespawnMapTypeId;
            return true;
        }
    }
}

