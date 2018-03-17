using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;

namespace OpenNos.DAL.Mock
{
    public class ShellEffectDAO : BaseDAO<ShellEffectDTO>, IShellEffectDAO
    {
        #region Methods

        public DeleteResult DeleteByEquipmentSerialId(Guid id) => throw new NotImplementedException();

        public ShellEffectDTO InsertOrUpdate(ShellEffectDTO shelleffect) => throw new NotImplementedException();

        public void InsertOrUpdateFromList(List<ShellEffectDTO> shellEffects, Guid equipmentSerialId) => throw new NotImplementedException();

        public IEnumerable<ShellEffectDTO> LoadByEquipmentSerialId(Guid id) => throw new NotImplementedException();

        #endregion
    }
}