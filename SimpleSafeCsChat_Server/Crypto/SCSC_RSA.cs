using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server
{
    class SCSC_RSA
    {
        RSACryptoServiceProvider rsaPersonal;
        RSACryptoServiceProvider rsaTarget;
        public SCSC_RSA()
        {
            rsaPersonal = new RSACryptoServiceProvider();
            rsaPersonal.FromXmlString(ServerSpecificStrings.GetServerRsaPrivateKey());
            GetPersonalPublicKey();
            GetPrivateKey();
        }

        public string GetPersonalPublicKey()
        {
            string publicKey = rsaPersonal.ToXmlString(false);
            return publicKey;
        }
        
       public string GetPrivateKey()
        {
            string privateKey = rsaPersonal.ToXmlString(true);
            return privateKey;
        }

        public byte[] EncryptTarget(byte[] text)
        {
            return RSAEncrypt(text, rsaTarget.ExportParameters(false), false);
        }

        public byte[] DecryptPersonal(byte[] encrypted)
        {
            return RSADecrypt(encrypted, rsaPersonal, false);
        }

        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Debug.WriteLine(e.Message);

                return null;
            }
        }

        public static byte[] RSADecrypt(byte[] DataToDecrypt, RSACryptoServiceProvider RSA, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Debug.WriteLine(e.ToString());

                return null;
            }
        }
    }
}
