using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCSChat
{
    public enum chatManagerError
    {
        noError = 0,
        failedToAuth,
        userAlreadyExists
    }
    class ServerConnectionHelper
    {
        
        // static functions
        //This function contains server-specific strings
        public static ChatManager TryRegister(Networker networker, string login, string password, out chatManagerError error)
        {
            error = chatManagerError.userAlreadyExists;
            ChatManager chatManager = TryStartChatManager(networker, String.Format("{0}&login={1}&password={2}", ServerSpecificStrings.MesageTypeRegister, login, password));
            if (chatManager != null)
            {
                error = chatManagerError.noError;
            }
            return chatManager;
        }

        //This function contains server-specific strings
        public static ChatManager TryAuth(Networker networker, string login, string password, out chatManagerError error)
        {
            error = chatManagerError.failedToAuth;
            ChatManager chatManager = TryStartChatManager(networker, String.Format("{0}&login={1}&password={2}", ServerSpecificStrings.MesageTypeAuth, login, password));
            if (chatManager != null)
            {
                chatManager.AddUserCredintals(login, password);
                error = chatManagerError.noError;
            }
            return chatManager;
        }

        //Trying to get positive response from server, if all OK, then create ChatManager
        //This function contains server-specific strings
        public static ChatManager TryStartChatManager(Networker networker, string serverRequest)
        {
            CryptoManager cryptoManager = Handshake(networker);
            if (cryptoManager != null)
            {
                //message = serverRequest
                string response = ChatManager.SendMessageAndWaitResponseCrypted(serverRequest, networker, cryptoManager);
                if (response.Equals(ServerSpecificStrings.MesageOk))
                {
                    return new ChatManager(networker, cryptoManager);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        //Estabilish crypto tunnel and save session keys to CryptoManager object
        static CryptoManager Handshake(Networker networker)
        {
            CryptoManager cryptoManager = null;

            string AESKey = CryptoManager.GetAESKeyB64();
            string response = networker.SendMessageAndWaitResponse(
                CryptoManager.EncryptRSA2B64("message=handshake&key=" + AESKey)
                );
            if (!String.IsNullOrEmpty(response))
            {
                CryptoManager cryptoManagerTest = new CryptoManager(AESKey, response);
                string responseConnected = ChatManager.SendMessageAndWaitResponseCrypted(ServerSpecificStrings.MesageOk, networker, cryptoManagerTest);
                if (responseConnected.Equals(ServerSpecificStrings.MesageOk))
                {
                    cryptoManager = cryptoManagerTest;
                }
            }
            return cryptoManager;
        }
    }
}
