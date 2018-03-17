using OpenNos.DAL.EF;
using OpenNos.Data;
using System;

namespace OpenNos.Mapper.Mappers
{
    public class MapTypeMapper
    {
        public MapTypeMapper()
        {

        }

        public bool ToMapTypeDTO(MapType input, MapTypeDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.MapTypeId = input.MapTypeId;
            output.MapTypeName = input.MapTypeName;
            output.PotionDelay = input.PotionDelay;
            output.RespawnMapTypeId = input.RespawnMapTypeId;
            output.ReturnMapTypeId = input.ReturnMapTypeId;
            return true;
        }

        public bool ToMapType(MapTypeDTO input, MapType output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.MapTypeId = input.MapTypeId;
            output.MapTypeName = input.MapTypeName;
            output.PotionDelay = input.PotionDelay;
            output.RespawnMapTypeId = input.RespawnMapTypeId;
            output.ReturnMapTypeId = input.ReturnMapTypeId;
            return true;
        }
    }
}
