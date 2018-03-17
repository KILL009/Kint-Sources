using OpenNos.SCS.Communication.ScsServices.Service;
using OpenNos.Master.Library.Data;
using System;

namespace OpenNos.Master.Library.Interface
{
    [ScsService(Version = "1.1.0.0")]
    public interface IConfigurationService
    {
        /// <summary>
        /// Authenticates a Client to the Service
        /// </summary>
        /// <param name="authKey">The private Authentication key</param>
        /// <param name="serverId"></param>
        /// <returns>true if successful, else false</returns>
        bool Authenticate(string authKey, Guid serverId);

        /// <summary>
        /// Get the Configuration Object from the Service
        /// </summary>
        /// <returns></returns>
        ConfigurationObject GetConfigurationObject();

        /// <summary>
        /// Update the Configuration Object to the Service
        /// </summary>
        /// <param name="configurationObject"></param>
        void UpdateConfigurationObject(ConfigurationObject configurationObject);
    }
}