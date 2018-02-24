/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class QuestDAO : IQuestDAO
    {
        #region Methods


        public DeleteResult DeleteById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Quest deleteEntity = context.Quest.Find(id);
                    if (deleteEntity != null)
                    {
                        context.Quest.Remove(deleteEntity);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), id, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public QuestDTO InsertOrUpdate(QuestDTO quest)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Quest entity = context.Quest.Find(quest.QuestId);

                    if (entity == null)
                    {
                        return insert(quest, context);
                    }
                    return update(entity, quest, context);
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), quest, e.Message), e);
                return quest;
            }
        }

        public void InsertOrUpdateFromList(List<QuestDTO> questList)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    void insert(QuestDTO quest)
                    {
                        Quest _entity = new Quest();
                        Mapper.Mapper.Instance.QuestMapper.ToQuest(quest, _entity);
                        context.Quest.Add(_entity);
                    }

                    void update(Quest _entity, QuestDTO quest)
                    {
                        if (_entity != null)
                        {
                            Mapper.Mapper.Instance.QuestMapper.ToQuest(quest, _entity);
                            context.SaveChanges();
                        }
                    }

                    foreach (QuestDTO item in questList)
                    {
                        Quest entity = context.Quest.Find(item.QuestId);

                        if (entity == null)
                        {
                            insert(item);
                        }
                        else
                        {
                            update(entity, item);
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<QuestDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<QuestDTO> result = new List<QuestDTO>();
                foreach(Quest entity in context.Quest)
                {
                    QuestDTO dto = new QuestDTO();
                    Mapper.Mapper.Instance.QuestMapper.ToQuestDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public QuestDTO LoadById(long id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                QuestDTO dto = new QuestDTO();
                if(Mapper.Mapper.Instance.QuestMapper.ToQuestDTO(context.Quest.Find(id), dto))
                {
                    return dto;
                }

                return null;
            }
        }

        private QuestDTO insert(QuestDTO quest, OpenNosContext context)
        {
            Quest entity = new Quest();
            Mapper.Mapper.Instance.QuestMapper.ToQuest(quest, entity);
            context.Quest.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.QuestMapper.ToQuestDTO(entity, quest))
            {
                return quest;
            }

            return null;
        }

        private QuestDTO update(Quest entity, QuestDTO quest, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.QuestMapper.ToQuest(quest, entity);
                context.SaveChanges();
            }

            if(Mapper.Mapper.Instance.QuestMapper.ToQuestDTO(entity, quest))
            {
                return quest;
            }

            return null;
        }

        #endregion
    }
}