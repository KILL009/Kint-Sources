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
    public class QuestProgressDAO : IQuestProgressDAO
    {
        #region Methods

        public DeleteResult DeleteById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    QuestProgress deleteEntity = context.QuestProgress.FirstOrDefault(s => s.QuestProgressId == id);
                    if (deleteEntity != null)
                    {
                        context.QuestProgress.Remove(deleteEntity);
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

        public QuestProgressDTO InsertOrUpdate(QuestProgressDTO questProgress)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    QuestProgress entity = context.QuestProgress.FirstOrDefault(s => s.QuestProgressId == questProgress.QuestProgressId);

                    if (entity == null)
                    {
                        return insert(questProgress, context);
                    }
                    return update(entity, questProgress, context);
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), questProgress, e.Message), e);
                return questProgress;
            }
        }

        public void InsertOrUpdateFromList(List<QuestProgressDTO> questProgressList)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    void insert(QuestProgressDTO quest)
                    {
                        QuestProgress _entity = new QuestProgress();
                        Mapper.Mapper.Instance.QuestProgressMapper.ToQuestProgress(quest, _entity);
                        context.QuestProgress.Add(_entity);
                    }

                    void update(QuestProgress _entity, QuestProgressDTO quest)
                    {
                        if (_entity != null)
                        {
                            Mapper.Mapper.Instance.QuestProgressMapper.ToQuestProgress(quest, _entity);
                            context.SaveChanges();
                        }
                    }

                    foreach (QuestProgressDTO item in questProgressList)
                    {
                        QuestProgress entity = context.QuestProgress.FirstOrDefault(s => s.QuestProgressId == item.QuestProgressId);

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

        public IEnumerable<QuestProgressDTO> LoadByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    List<QuestProgressDTO> result = new List<QuestProgressDTO>();
                    foreach(QuestProgress entity in context.QuestProgress.Where(s => s.CharacterId == characterId))
                    {
                        QuestProgressDTO dto = new QuestProgressDTO();
                        Mapper.Mapper.Instance.QuestProgressMapper.ToQuestProgressDTO(entity, dto);
                        result.Add(dto);
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public QuestProgressDTO LoadById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    QuestProgressDTO dto = new QuestProgressDTO();
                    if(Mapper.Mapper.Instance.QuestProgressMapper.ToQuestProgressDTO(context.QuestProgress.FirstOrDefault(s => s.QuestProgressId == id), dto))
                    {
                        return dto;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private QuestProgressDTO insert(QuestProgressDTO quest, OpenNosContext context)
        {
            QuestProgress entity = new QuestProgress();
            Mapper.Mapper.Instance.QuestProgressMapper.ToQuestProgress(quest,entity);
            context.QuestProgress.Add(entity);
            context.SaveChanges();
            if(Mapper.Mapper.Instance.QuestProgressMapper.ToQuestProgressDTO(entity, quest))
            {
                return quest;
            }

            return null;
        }

        private QuestProgressDTO update(QuestProgress entity, QuestProgressDTO quest, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.QuestProgressMapper.ToQuestProgress(quest, entity);
                context.SaveChanges();
            }

            if(Mapper.Mapper.Instance.QuestProgressMapper.ToQuestProgressDTO(entity, quest))
            {
                return quest;
            }

            return null;
        }

        #endregion
    }
}