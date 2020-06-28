using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCSChat
{
    class SCSC_RSA
    {
        //RSACryptoServiceProvider rsaPersonal;
        RSACryptoServiceProvider rsaTarget;
        public SCSC_RSA()
        {
            rsaTarget = new RSACryptoServiceProvider();
            rsaTarget.FromXmlString(ServerSpecificStrings.GetServerRsaPublicKey());
            //GetPersonalPublicKey();
            //GetPrivateKey();
        }

        public string GetPersonalPublicKey()
        {
            string publicKey = rsaTarget.ToXmlString(false);
            return publicKey;
        }

        public string GetPrivateKey()
        {
            string privateKey = rsaTarget.ToXmlString(true);
            return privateKey;
        }

        public byte[] EncryptTarget(byte[] text)
        {
            return RSAEncrypt(text, rsaTarget, false);
        }

        //public byte[] DecryptPersonal(byte[] encrypted)
        //{
        //    return RSADecrypt(encrypted, rsaPersonal, true);
        //}

        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSACryptoServiceProvider RSA, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
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
