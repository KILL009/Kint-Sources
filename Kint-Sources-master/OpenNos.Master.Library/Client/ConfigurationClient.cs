using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using System.Threading.Tasks;

namespace OpenNos.Master.Library.Client
{
    internal  class ConfigurationClient : IConfigurationClient
    {
        public void ConfigurationUpdated(ConfigurationObject configurationObject) => Task.Run(() => ConfigurationServiceClient.Instance.OnConfigurationUpdated(configurationObject));
    }
}
