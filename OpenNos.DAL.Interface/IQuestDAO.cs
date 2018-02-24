using OpenNos.Data;
using OpenNos.Data.Enums;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IQuestDAO
    {
        #region Methods

        DeleteResult DeleteById(long id);

        QuestDTO InsertOrUpdate(QuestDTO quest);

        void InsertOrUpdateFromList(List<QuestDTO> questList);

        QuestDTO LoadById(long id);

        IEnumerable<QuestDTO> LoadAll();

        #endregion
    }
}