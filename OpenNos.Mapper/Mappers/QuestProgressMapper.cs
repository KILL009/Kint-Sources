using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class QuestProgressMapper
    {
        public QuestProgressMapper()
        {
        }

        public bool ToQuestProgressDTO(QuestProgress input, QuestProgressDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.IsFinished = input.IsFinished;
            output.QuestData = input.QuestData;
            output.QuestId = input.QuestId;
            output.QuestProgressId = input.QuestProgressId;
            return true;
        }

        public bool ToQuestProgress(QuestProgressDTO input, QuestProgress output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.CharacterId = input.CharacterId;
            output.IsFinished = input.IsFinished;
            output.QuestData = input.QuestData;
            output.QuestId = input.QuestId;
            output.QuestProgressId = input.QuestProgressId;
            return true;
        }
    }
}

