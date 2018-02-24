using OpenNos.DAL.Interface;
using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Mock
{
    public class QuestProgressDAO : IQuestProgressDAO
    {
        public DeleteResult DeleteById(long id) => throw new NotImplementedException();

        public void InitializeMapper() => throw new NotImplementedException();

        public QuestProgressDTO InsertOrUpdate(QuestProgressDTO questProgress) => throw new NotImplementedException();

        public void InsertOrUpdateFromList(List<QuestProgressDTO> questProgressList) => throw new NotImplementedException();

        public IEnumerable<QuestProgressDTO> LoadByCharacterId(long characterId) => throw new NotImplementedException();

        public QuestProgressDTO LoadById(long id) => throw new NotImplementedException();
    }
}
