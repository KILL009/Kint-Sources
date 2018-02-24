using OpenNos.Master.Library.Data;

namespace OpenNos.Master.Library.Interface
{
    public interface IConfigurationClient
    {
        #region Methods

        void ConfigurationUpdated(ConfigurationObject configurationObject);

        #endregion
    }
}