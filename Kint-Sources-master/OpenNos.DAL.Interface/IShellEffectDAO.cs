using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IShellEffectDAO
    {
        #region Methods

        DeleteResult DeleteByEquipmentSerialId(Guid id);

        ShellEffectDTO InsertOrUpdate(ShellEffectDTO shelleffect);

        void InsertOrUpdateFromList(List<ShellEffectDTO> shellEffects, Guid equipmentSerialId);

        IEnumerable<ShellEffectDTO> LoadByEquipmentSerialId(Guid id);

        #endregion
    }
}