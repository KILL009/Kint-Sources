using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;

namespace OpenNos.DAL.Mock
{
    public class MaintenanceLogDAO : BaseDAO<MaintenanceLogDTO>, IMaintenanceLogDAO
    {
        #region Methods

        public MaintenanceLogDTO LoadFirst() => throw new NotImplementedException();

        #endregion
    }
}