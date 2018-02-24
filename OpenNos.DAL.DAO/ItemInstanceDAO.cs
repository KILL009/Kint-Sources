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
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class ItemInstanceDAO : IItemInstanceDAO
    {
        #region Methods

        public virtual DeleteResult Delete(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                ItemInstance entity = context.Set<ItemInstance>().FirstOrDefault(i => i.Id == id);
                if (entity != null)
                {
                    context.Set<ItemInstance>().Remove(entity);
                    context.SaveChanges();
                }
                return DeleteResult.Deleted;
            }
        }

        public DeleteResult DeleteFromSlotAndType(long characterId, short slot, InventoryType type)
        {
            try
            {
                ItemInstanceDTO dto = LoadBySlotAndType(characterId, slot, type);
                if (dto != null)
                {
                    return Delete(dto.Id);
                }

                return DeleteResult.Unknown;
            }
            catch (Exception e)
            {
                Logger.Error($"characterId: {characterId} slot: {slot} type: {type}", e);
                return DeleteResult.Error;
            }
        }

        public DeleteResult DeleteGuidList(IEnumerable<Guid> guids)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                try
                {
                    foreach (Guid id in guids)
                    {
                        ItemInstance entity = context.ItemInstance.FirstOrDefault(i => i.Id == id);
                        if (entity != null)
                        {
                            context.ItemInstance.Remove(entity);
                        }
                    }
                    context.SaveChanges();
                }
                catch
                {
                    foreach (Guid id in guids)
                    {
                        try
                        {
                            Delete(id);
                        }
                        catch (Exception ex)
                        {
                            // TODO: Work on: statement conflicted with the REFERENCE constraint
                            //       "FK_dbo.BazaarItem_dbo.ItemInstance_ItemInstanceId". The
                            //       conflict occurred in database "opennos", table "dbo.BazaarItem",
                            //       column 'ItemInstanceId'.
                            Logger.LogUserEventError("ONSAVEDELETION_EXCEPTION", "Saving Process", $"Detailed Item Information: Item ID = {id}", ex);
                        }
                    }
                }
                return DeleteResult.Deleted;
            }
        }

        public IEnumerable<ItemInstanceDTO> InsertOrUpdate(IEnumerable<ItemInstanceDTO> dtos)
        {
            try
            {
                IList<ItemInstanceDTO> results = new List<ItemInstanceDTO>();
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ItemInstanceDTO dto in dtos)
                    {
                        results.Add(InsertOrUpdate(context, dto));
                    }
                }
                return results;
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return Enumerable.Empty<ItemInstanceDTO>();
            }
        }

        public ItemInstanceDTO InsertOrUpdate(ItemInstanceDTO dto)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return InsertOrUpdate(context, dto);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return null;
            }
        }

        public SaveResult InsertOrUpdateFromList(IEnumerable<ItemInstanceDTO> items)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    void insert(ItemInstanceDTO iteminstance)
                    {
                        ItemInstance _entity = new ItemInstance();
                        map(iteminstance, _entity);
                        context.ItemInstance.Add(_entity);
                        context.SaveChanges();
                        iteminstance.Id = _entity.Id;
                    }

                    void update(ItemInstance _entity, ItemInstanceDTO iteminstance)
                    {
                        if (_entity != null)
                        {
                            map(iteminstance, _entity);
                        }
                    }

                    foreach (ItemInstanceDTO item in items)
                    {
                        ItemInstance entity = context.ItemInstance.FirstOrDefault(c => c.Id == item.Id);

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
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<ItemInstanceDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<ItemInstanceDTO> result = new List<ItemInstanceDTO>();
                foreach (ItemInstance itemInstance in context.ItemInstance.Where(i => i.CharacterId.Equals(characterId)))
                {
                    ItemInstanceDTO output = new ItemInstanceDTO();
                    map(itemInstance, output);
                    result.Add(output);
                }
                return result;
            }
        }

        public ItemInstanceDTO LoadById(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                ItemInstanceDTO ItemInstanceDTO = new ItemInstanceDTO();
                if (map(context.ItemInstance.FirstOrDefault(i => i.Id.Equals(id)), ItemInstanceDTO))
                {
                    return ItemInstanceDTO;
                }

                return null;
            }
        }

        public ItemInstanceDTO LoadBySlotAndType(long characterId, short slot, InventoryType type)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    ItemInstance entity = context.ItemInstance.FirstOrDefault(i => i.CharacterId == characterId && i.Slot == slot && i.Type == type);
                    ItemInstanceDTO output = new ItemInstanceDTO();
                    if (map(entity, output))
                    {
                        return output;
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

        public IEnumerable<ItemInstanceDTO> LoadByType(long characterId, InventoryType type)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<ItemInstanceDTO> result = new List<ItemInstanceDTO>();
                foreach (ItemInstance itemInstance in context.ItemInstance.Where(i => i.CharacterId == characterId && i.Type == type))
                {
                    ItemInstanceDTO output = new ItemInstanceDTO();
                    map(itemInstance, output);
                    result.Add(output);
                }
                return result;
            }
        }

        public IList<Guid> LoadSlotAndTypeByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.ItemInstance.Where(i => i.CharacterId.Equals(characterId)).Select(i => i.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        protected ItemInstanceDTO Insert(ItemInstanceDTO dto, OpenNosContext context)
        {
            ItemInstance entity = new ItemInstance();
            map(dto, entity);
            context.Set<ItemInstance>().Add(entity);
            context.SaveChanges();
            if (map(entity, dto))
            {
                return dto;
            }

            return null;
        }

        protected ItemInstanceDTO InsertOrUpdate(OpenNosContext context, ItemInstanceDTO dto)
        {
            try
            {
                ItemInstance entity = context.ItemInstance.FirstOrDefault(c => c.Id == dto.Id);
                dto = entity == null ? Insert(dto, context) : Update(entity, dto, context);
                return dto;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        protected ItemInstanceDTO Update(ItemInstance entity, ItemInstanceDTO inventory, OpenNosContext context)
        {
            if (entity != null)
            {
                map(inventory, entity, true);
                context.SaveChanges();
            }
            if (map(entity, inventory))
            {
                return inventory;
            }

            return null;
        }

        private bool map(ItemInstance input, ItemInstanceDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            Mapper.Mapper.Instance.ItemInstanceMapper.ToItemInstanceDTO(input, output);
            if (output.EquipmentSerialId == Guid.Empty)
            {
                output.EquipmentSerialId = Guid.NewGuid();
            }
            return true;
        }

        private bool map(ItemInstanceDTO input, ItemInstance output, bool exists = false)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            Mapper.Mapper.Instance.ItemInstanceMapper.ToItemInstance(input, output);
            if (output.EquipmentSerialId == Guid.Empty)
            {
                output.EquipmentSerialId = Guid.NewGuid();
            }
            return true;
        }

        #endregion
    }
}