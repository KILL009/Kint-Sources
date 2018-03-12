using System;

namespace OpenNos.Data
{
    public class EquipmentOptionDTO : SynchronizableBaseDTO
    {
        #region Properties

        public byte Level { get; set; }

        public byte Type { get; set; }

        public int Value { get; set; }

        public Guid WearableInstanceId { get; set; }

        #endregion
    }
}