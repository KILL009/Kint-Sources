using OpenNos.DAL.EF;
using OpenNos.Data;
using System;

namespace OpenNos.Mapper.Mappers
{
    public class MapTypeMapMapper
    {
        public MapTypeMapMapper()
        {

        }

        public bool ToMapTypeMapDTO(MapTypeMap input, MapTypeMapDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.MapId = input.MapId;
            output.MapTypeId = input.MapTypeId;
            return true;
        }

        public bool ToMapTypeMap(MapTypeMapDTO input, MapTypeMap output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.MapId = input.MapId;
            output.MapTypeId = input.MapTypeId;
            return true;
        }
    }
}
