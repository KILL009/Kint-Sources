using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class PortalMapper
    {
        public PortalMapper()
        {
        }

        public bool ToPortalDTO(Portal input, PortalDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.DestinationMapId = input.DestinationMapId;
            output.DestinationX = input.DestinationX;
            output.DestinationY = input.DestinationY;
            output.IsDisabled = input.IsDisabled;
            output.PortalId = input.PortalId;
            output.SourceMapId = input.SourceMapId;
            output.SourceX = input.SourceX;
            output.SourceY = input.SourceY;
            output.Type = input.Type;
            return true;
        }

        public bool ToPortal(PortalDTO input, Portal output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.DestinationMapId = input.DestinationMapId;
            output.DestinationX = input.DestinationX;
            output.DestinationY = input.DestinationY;
            output.IsDisabled = input.IsDisabled;
            output.PortalId = input.PortalId;
            output.SourceMapId = input.SourceMapId;
            output.SourceX = input.SourceX;
            output.SourceY = input.SourceY;
            output.Type = input.Type;
            return true;
        }
    }
}

