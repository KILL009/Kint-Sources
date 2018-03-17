using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class PenaltyLogMapper
    {
        public PenaltyLogMapper()
        {
        }

        public bool ToPenaltyLogDTO(PenaltyLog input, PenaltyLogDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.AccountId = input.AccountId;
            output.AdminName = input.AdminName;
            output.DateEnd = input.DateEnd;
            output.DateStart = input.DateStart;
            output.Penalty = input.Penalty;
            output.PenaltyLogId = input.PenaltyLogId;
            output.Reason = input.Reason;
            return true;
        }

        public bool ToPenaltyLog(PenaltyLogDTO input, PenaltyLog output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.AccountId = input.AccountId;
            output.AdminName = input.AdminName;
            output.DateEnd = input.DateEnd;
            output.DateStart = input.DateStart;
            output.Penalty = input.Penalty;
            output.PenaltyLogId = input.PenaltyLogId;
            output.Reason = input.Reason;
            return true;
        }
    }
}
