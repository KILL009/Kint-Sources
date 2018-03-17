using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class FamilyLogMapper
    {
        public FamilyLogMapper()
        {

        }

        public bool ToFamilyLogDTO(FamilyLog input, FamilyLogDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.FamilyId = input.FamilyId;
            output.FamilyLogData = input.FamilyLogData;
            output.FamilyLogId = input.FamilyLogId;
            output.FamilyLogType = input.FamilyLogType;
            output.Timestamp = input.Timestamp;
            return true;
        }

        public bool ToFamilyLog(FamilyLogDTO input, FamilyLog output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.FamilyId = input.FamilyId;
            output.FamilyLogData = input.FamilyLogData;
            output.FamilyLogId = input.FamilyLogId;
            output.FamilyLogType = input.FamilyLogType;
            output.Timestamp = input.Timestamp;
            return true;
        }
    }
}
