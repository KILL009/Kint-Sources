using System;

namespace OpenNos.Data
{
    public interface IItemInstance
    {
        #region Properties

        byte Amount { get; set; }

        long? BoundCharacterId { get; set; }

        short Design { get; set; }

        Guid Id { get; set; }

        DateTime? ItemDeleteTime { get; set; }

        short ItemVNum { get; set; }

        sbyte Rare { get; set; }

        byte Upgrade { get; set; }

        #endregion
    }
}