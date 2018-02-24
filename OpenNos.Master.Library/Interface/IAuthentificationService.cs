using OpenNos.SCS.Communication.ScsServices.Service;
using OpenNos.Data;
using OpenNos.Master.Library.Data;
using System.Collections.Generic;

namespace OpenNos.Master.Library.Interface
{
    [ScsService(Version = "1.1.0.0")]
    public interface IAuthentificationService
    {
        /// <summary>
        /// Authenticates a Client to the Service
        /// </summary>
        /// <param name="authKey">The private Authentication key</param>
        /// <returns>true if successful, else false</returns>
        bool Authenticate(string authKey);

        /// <summary>
        /// Checks if the given Credentials are Valid
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passHash"></param>
        /// <returns></returns>
        AccountDTO ValidateAccount(string userName, string passHash);

        /// <summary>
        /// Checks if the given Credentials are Valid and return the CharacterDTO
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="characterName"></param>
        /// <param name="passHash"></param>
        /// <returns></returns>
        CharacterDTO ValidateAccountAndCharacter(string userName, string characterName, string passHash);
    }
}