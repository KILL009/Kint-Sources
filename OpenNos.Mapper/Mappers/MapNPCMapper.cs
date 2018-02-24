using OpenNos.DAL.EF;
using OpenNos.Data;
using System;

namespace OpenNos.Mapper.Mappers
{
    public class MapNPCMapper
    {
        public MapNPCMapper()
        {

        }

        public bool ToMapNPCDTO(MapNpc input, MapNpcDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Dialog = input.Dialog;
            output.Effect = input.Effect;
            output.EffectDelay = input.EffectDelay;
            output.IsDisabled = input.IsDisabled;
            output.IsMoving = input.IsMoving;
            output.IsSitting = input.IsSitting;
            output.MapId = input.MapId;
            output.MapNpcId = input.MapNpcId;
            output.MapX = input.MapX;
            output.MapY = input.MapY;
            output.NpcVNum = input.NpcVNum;
            output.Position = input.Position;
            return true;
        }

        public bool ToMapNPC(MapNpcDTO input, MapNpc output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Dialog = input.Dialog;
            output.Effect = input.Effect;
            output.EffectDelay = input.EffectDelay;
            output.IsDisabled = input.IsDisabled;
            output.IsMoving = input.IsMoving;
            output.IsSitting = input.IsSitting;
            output.MapId = input.MapId;
            output.MapNpcId = input.MapNpcId;
            output.MapX = input.MapX;
            output.MapY = input.MapY;
            output.NpcVNum = input.NpcVNum;
            output.Position = input.Position;
            return true;
        }
    }
}
