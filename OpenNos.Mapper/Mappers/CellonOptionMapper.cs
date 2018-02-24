
using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class CellonOptionMapper
    {
        public CellonOptionMapper()
        {

        }

        public bool ToCellonOptionDTO(CellonOption input, CellonOptionDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CellonOptionId = input.CellonOptionId;
            output.EquipmentSerialId = input.EquipmentSerialId;
            output.Level = input.Level;
            output.Type = input.Type;
            output.Value = input.Value;
            return true;
        }

        public bool ToCellonOption(CellonOptionDTO input, CellonOption output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CellonOptionId = input.CellonOptionId;
            output.EquipmentSerialId = input.EquipmentSerialId;
            output.Level = input.Level;
            output.Type = input.Type;
            output.Value = input.Value;
            return true;
        }
    }
}
