using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class RespawnMapper
    {
        public RespawnMapper()
        {
        }

        public bool ToRespawnDTO(Respawn input, RespawnDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.MapId = input.MapId;
            output.RespawnId = input.RespawnId;
            output.RespawnMapTypeId = input.RespawnMapTypeId;
            output.X = input.X;
            output.Y = input.Y;
            return true;
        }

        public bool ToRespawn(RespawnDTO input, Respawn output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.MapId = input.MapId;
            output.RespawnId = input.RespawnId;
            output.RespawnMapTypeId = input.RespawnMapTypeId;
            output.X = input.X;
            output.Y = input.Y;
            return true;
        }
    }
}

