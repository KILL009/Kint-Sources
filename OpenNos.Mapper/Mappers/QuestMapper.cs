using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class QuestMapper
    {
        public QuestMapper()
        {
        }

        public bool ToQuestDTO(Quest input, QuestDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.QuestData = input.QuestData;
            output.QuestId = input.QuestId;
            return true;
        }

        public bool ToQuest(QuestDTO input, Quest output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.QuestData = input.QuestData;
            output.QuestId = input.QuestId;
            return true;
        }
    }
}

