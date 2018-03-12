using System;
using System.Text;

namespace OpenNos.Core
{
    public class LoginEncryption : EncryptionBase
    {
        #region Instantiation

        public LoginEncryption() : base(false)
        {
        }

        #endregion

        #region Methods

        public override string Decrypt(byte[] packet, int sessionId = 0)
        {
            try
            {
                var decryptedPacket = string.Empty;

                foreach (byte character in packet)
                {
                    if (character > 14)
                    {
                        decryptedPacket += Convert.ToChar((character - 15) ^ 195);
                    }
                    else
                    {
                        decryptedPacket += Convert.ToChar((256 - (15 - character)) ^ 195);
                    }
                }

                return decryptedPacket;
            }
            catch
            {
                return string.Empty;
            }
        }

        public override string DecryptCustomParameter(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] Encrypt(string packet)
        {
            try
            {
                packet += " ";
                byte[] tmp = Encoding.Default.GetBytes(packet);
                for (int i = 0; i < packet.Length; i++)
                    tmp[i] = Convert.ToByte(tmp[i] + 15);

                tmp[tmp.Length - 1] = 25;
                return tmp;
            }
            catch
            {
                return new byte[0];
            }
        }

        #endregion
    }
}