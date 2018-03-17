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
    public class MailDAO : IMailDAO
    {
        #region Methods

        public DeleteResult DeleteById(long mailId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Mail mail = context.Mail.First(i => i.MailId.Equals(mailId));

                    if (mail != null)
                    {
                        context.Mail.Remove(mail);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref MailDTO mail)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long mailId = mail.MailId;
                    Mail entity = context.Mail.FirstOrDefault(c => c.MailId.Equals(mailId));

                    if (entity == null)
                    {
                        mail = insert(mail, context);
                        return SaveResult.Inserted;
                    }

                    mail.MailId = entity.MailId;
                    mail = update(entity, mail, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MailDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MailDTO> result = new List<MailDTO>();
                foreach (Mail mail in context.Mail)
                {
                    MailDTO dto = new MailDTO();
                    Mapper.Mapper.Instance.MailMapper.ToMailDTO(mail, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public MailDTO LoadById(long mailId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MailDTO dto = new MailDTO();
                    if(Mapper.Mapper.Instance.MailMapper.ToMailDTO(context.Mail.FirstOrDefault(i => i.MailId.Equals(mailId)), dto))
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

        public IEnumerable<MailDTO> LoadSentByCharacter(long characterId)
        {
            //Where(s => s.SenderId == CharacterId && s.IsSenderCopy && MailList.All(m => m.Value.MailId != s.MailId))
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MailDTO> result = new List<MailDTO>();
                foreach (Mail mail in context.Mail.Where(s => s.SenderId == characterId && s.IsSenderCopy).Take(40))
                {
                    MailDTO dto = new MailDTO();
                    Mapper.Mapper.Instance.MailMapper.ToMailDTO(mail, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<MailDTO> LoadSentToCharacter(long characterId)
        {
            //s => s.ReceiverId == CharacterId && !s.IsSenderCopy && MailList.All(m => m.Value.MailId != s.MailId)).Take(50)
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<MailDTO> result = new List<MailDTO>();
                foreach (Mail mail in context.Mail.Where(s => s.ReceiverId == characterId && !s.IsSenderCopy).Take(40))
                {
                    MailDTO dto = new MailDTO();
                    Mapper.Mapper.Instance.MailMapper.ToMailDTO(mail, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        private MailDTO insert(MailDTO mail, OpenNosContext context)
        {
            try
            {
                Mail entity = new Mail();
                Mapper.Mapper.Instance.MailMapper.ToMail(mail, entity);
                context.Mail.Add(entity);
                context.SaveChanges();
                if(Mapper.Mapper.Instance.MailMapper.ToMailDTO(entity, mail))
                {
                    return mail;
                }

                return null;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        Logger.Error(new InvalidOperationException(message, raise));
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private MailDTO update(Mail entity, MailDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.MailMapper.ToMail(respawn, entity);
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.MailMapper.ToMailDTO(entity, respawn))
            {
                return respawn;
            }

            return null;
        }

        #endregion
    }
}