using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace SimpleSafeCSChat
{
    class SCSC_AES
    {
        public static byte[] GetSessionKey()
        {
            return Aes.Create().Key;
        }
        //AES256 with key from SHA256 of user's password used for encrypt/decrypt user data
        public static byte[] Encrypt(string text, byte[] password, ref byte[] IV)
        {
            byte[] encrypted = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = password;
                if (IV == null)
                {
                    IV = aesAlg.IV;
                }
                else
                {
                    aesAlg.IV = IV;
                }

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        //AES256 with key from SHA256 of user's password used for encrypt/decrypt user data
        public static string Decrypt(byte[] encrypted, byte[] password, byte[] IV)
        {
            string text = "";
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = password;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            text = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return text;
        }
    }
}
