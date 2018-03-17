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
using OpenNos.Domain;
using System;
using System.Data.Entity;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class AccountDAO : IAccountDAO
    {
        #region Methods

        public DeleteResult Delete(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(c => c.AccountId.Equals(accountId));

                    if (account != null)
                    {
                        context.Account.Remove(account);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ACCOUNT_ERROR"), accountId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref AccountDTO account)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long accountId = account.AccountId;
                    Account entity = context.Account.FirstOrDefault(c => c.AccountId.Equals(accountId));

                    if (entity == null)
                    {
                        account = insert(account, context);
                        return SaveResult.Inserted;
                    }
                    account = update(entity, account, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_ACCOUNT_ERROR"), account.AccountId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public AccountDTO LoadById(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(a => a.AccountId.Equals(accountId));
                    if (account != null)
                    {
                        AccountDTO accountDTO = new AccountDTO();
                        if(Mapper.Mapper.Instance.AccountMapper.ToAccountDTO(account, accountDTO))
                        {
                            return accountDTO;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return null;
        }

        public AccountDTO LoadByName(string name)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Account account = context.Account.FirstOrDefault(a => a.Name.Equals(name));
                    if (account != null)
                    {
                        AccountDTO accountDTO = new AccountDTO();
                        if(Mapper.Mapper.Instance.AccountMapper.ToAccountDTO(account, accountDTO))
                        {
                            return accountDTO;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return null;
        }

        public void WriteGeneralLog(long accountId, string ipAddress, long? characterId, GeneralLogType logType, string logData)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    GeneralLog log = new GeneralLog
                    {
                        AccountId = accountId,
                        IpAddress = ipAddress,
                        Timestamp = DateTime.Now,
                        LogType = logType.ToString(),
                        LogData = logData,
                        CharacterId = characterId
                    };

                    context.GeneralLog.Add(log);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private AccountDTO insert(AccountDTO account, OpenNosContext context)
        {
            Account entity = new Account();
            Mapper.Mapper.Instance.AccountMapper.ToAccount(account, entity);
            context.Account.Add(entity);
            context.SaveChanges();
            Mapper.Mapper.Instance.AccountMapper.ToAccountDTO(entity, account);
            return account;
        }

        private AccountDTO update(Account entity, AccountDTO account, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mapper.Instance.AccountMapper.ToAccount(account, entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
            if(Mapper.Mapper.Instance.AccountMapper.ToAccountDTO(entity, account))
            {
                return account;
            }

            return null;
        }

        #endregion
    }
}