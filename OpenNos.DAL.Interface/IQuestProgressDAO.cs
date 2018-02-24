using OpenNos.Data;
using OpenNos.Data.Enums;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IQuestProgressDAO
    {
        #region Methods

        DeleteResult DeleteById(long id);

        QuestProgressDTO InsertOrUpdate(QuestProgressDTO questProgress);

        void InsertOrUpdateFromList(List<QuestProgressDTO> questProgressList);

        QuestProgressDTO LoadById(long id);

        IEnumerable<QuestProgressDTO> LoadByCharacterId(long characterId);

        #endregion
    }
}