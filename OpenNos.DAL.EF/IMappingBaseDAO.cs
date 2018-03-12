using System;

namespace OpenNos.DAL.EF
{
    public interface IMappingBaseDAO
    {
        #region Methods

        void InitializeMapper();

        IMappingBaseDAO RegisterMapping(Type gameObjectType);

        #endregion
    }
}