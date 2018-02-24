using OpenNos.DAL.Interface;
using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Mock
{
    public class QuestDAO : IQuestDAO
    {
        public DeleteResult DeleteById(long id) => throw new NotImplementedException();

        public void InitializeMapper() => throw new NotImplementedException();

        public QuestDTO InsertOrUpdate(QuestDTO quest) => throw new NotImplementedException();

        public void InsertOrUpdateFromList(List<QuestDTO> questList) => throw new NotImplementedException();

        public IEnumerable<QuestDTO> LoadAll() => throw new NotImplementedException();

        public QuestDTO LoadById(long id) => throw new NotImplementedException();
    }
}
