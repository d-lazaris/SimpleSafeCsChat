using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server
{
    class CryptoManager
    {
        int m_id;
        public int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        public static string DecryptRSAFromB64(string text)
        {
            SCSC_RSA rsa = new SCSC_RSA();
            byte[] textBytes = Convert.FromBase64String(text);
            byte[] encrypted = rsa.DecryptPersonal(textBytes);
            return Encoding.ASCII.GetString(encrypted);
        }

        public static string GetAESKeyB64(string password = "")
        {
            if (String.IsNullOrEmpty(password))
            {
                return Convert.ToBase64String(SCSC_AES.GetSessionKey());
            }
            else
            {
                return Convert.ToBase64String(sha256_hash(password));
            }
        }
        public CryptoManager(string sessionPasswordB64)
        {
            m_sessionPassword = Convert.FromBase64String(sessionPasswordB64);
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            m_sessionIV = sha256_hash(new string(Enumerable.Repeat(chars, random.Next(256)).Select(s => s[random.Next(s.Length)]).ToArray())).Take(16).ToArray();
        }
        public CryptoManager(string sessionPasswordB64, string sessionIVB64)
        {
            m_sessionPassword = Convert.FromBase64String(sessionPasswordB64);
            m_sessionIV = Convert.FromBase64String(sessionIVB64).Take(16).ToArray();
        }

        public CryptoManager()
        {
        }
        string m_user = "";
        //SHA265 from user name used for IV
        byte[] m_shaUser;
        byte[] m_shaPassword;
        byte[] m_sessionPassword;
        public byte[] m_sessionIV;
        public string EncryptWithSessionB64(string text)
        {
            byte[] IV = m_sessionIV;
            byte[] encMessage = SCSC_AES.Encrypt(text, m_sessionPassword, ref IV);
            byte[] im = new byte[IV.Length + encMessage.Length];
            //Copy from IV to output, IV size bytes (must be 32)
            Array.Copy(IV, im, IV.Length);
            Array.Copy(encMessage, 0, im, IV.Length, encMessage.Length);
            return Convert.ToBase64String(im);
        }

        public string DecryptWithSessionB64(string encryptedB64)
        {
            byte[] encrypted = Convert.FromBase64String(encryptedB64);
            byte[] IV = new byte[16];
            byte[] encryptedWOIV = new byte[encrypted.Length - 16];
            //Copy from encrypted to IV, IV size bytes (must be 32)
            Array.Copy(encrypted, IV, 16);
            //Copy from encrypted to encryptedWOIV[32], encrypted size bytes - IV size bytes (must be 32)
            Array.Copy(encrypted, 16, encryptedWOIV, 0, encrypted.Length - 16);
            return SCSC_AES.Decrypt(encryptedWOIV, m_sessionPassword, IV);
        }

        private static byte[] sha256_hash(string value)
        {
            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                return result;
            }
        }

        
        //AES256 with key from SHA256 of user's password used for encrypt/decrypt user data
        public string EncryptUserDataB64(string text)
        {
            return Convert.ToBase64String(SCSC_AES.Encrypt(text, m_shaPassword, ref m_shaUser));
        }

        //AES256 with key from SHA256 of user's password used for encrypt/decrypt user data
        public string DecryptUserDataB64(string encryptedB64)
        {
            byte[] encrypted = Convert.FromBase64String(encryptedB64);
            return SCSC_AES.Decrypt(encrypted, m_shaPassword, m_shaUser);
        }


        public void SetUser(string user, string password)
        {
            m_user = user;
            m_shaUser = sha256_hash(user).Take(16).ToArray();
            m_shaPassword = sha256_hash(password).Take(16).ToArray();
        }

        public string GetUser()
        {
            return m_user;
        }
        

    }
}

