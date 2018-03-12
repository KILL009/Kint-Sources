using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenNos.Core
{
    public abstract class EncryptionBase
    {
        #region Instantiation

        public EncryptionBase(bool hasCustomParameter)
        {
            HasCustomParameter = hasCustomParameter;
        }

        #endregion

        #region Properties

        public bool HasCustomParameter { get; set; }

        #endregion

        #region Methods

        public static string Sha512(string inputString)
        {
            using (SHA512 hash = SHA512.Create())
            {
                return string.Join(string.Empty, hash.ComputeHash(Encoding.UTF8.GetBytes(inputString)).Select(item => item.ToString("x2")));
            }
        }

        public abstract string Decrypt(byte[] data, int sessionId = 0);

        public abstract string DecryptCustomParameter(byte[] data);

        public abstract byte[] Encrypt(string data);

        #endregion
    }
}