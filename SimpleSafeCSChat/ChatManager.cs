using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleSafeCSChat.ViewModel;

namespace SimpleSafeCSChat
{

    public class ChatManager
    {
        Dictionary<string, CryptoManager> m_personalCryptoManagers;
        ChatScreenViewModel m_viewModel;
        CryptoManager m_cryptoManager;
        Networker m_networker;
        DiskManager diskManager;
        Thread m_serverCheckThread;
        bool threadStarted;
        string m_userName;
        private readonly object _lockSendingWhileUpdate = new object();

        public ChatManager(Networker networker, CryptoManager cryptoManager)
        {
            m_networker = networker;
            m_cryptoManager = cryptoManager;
            m_cryptoManager.Id = 0;
            m_personalCryptoManagers = new Dictionary<string, CryptoManager>();
            m_serverCheckThread = new Thread(ServerMessagesThread);
            m_serverCheckThread.Start();
        }

        string SendMessageAndWaitResponseCrypted(string message)
        {
            return SendMessageAndWaitResponseCrypted(message, m_cryptoManager);
        }

        string SendMessageAndWaitResponseCrypted(string message, CryptoManager cryptoManager)
        {
            return ChatManager.SendMessageAndWaitResponseCrypted(message, m_networker, cryptoManager);
        }
        public static string SendMessageAndWaitResponseCrypted(string message, Networker networker, CryptoManager cryptoManager)
        {
            string encryptedResponseB64 = networker.SendMessageAndWaitResponse(cryptoManager.EncryptWithSessionB64(message));
            return cryptoManager.DecryptWithSessionB64(encryptedResponseB64);
        }
        public bool AddChat(string chatname, string password)
        {
            if (String.IsNullOrEmpty(password))
            {
                string keyBased = CryptoManager.GetAESKeyB64();
                string response = SendMessageAndWaitResponseCrypted(ServerSpecificStrings.SendSessionKeyToUser(m_userName, chatname, keyBased));
                if (response.Equals(ServerSpecificStrings.MesageOk))
                {
                    return StartNewChatSession(chatname, keyBased);
                }
            }
            else
            {
                string response = SendMessageAndWaitResponseCrypted(ServerSpecificStrings.SendMessageToEnterGroup(m_userName, chatname, password));
                if (response.Equals(ServerSpecificStrings.MesageOk))
                {
                    return true;
                }
            }
            return false;

        }

        public void AddModel(ChatScreenViewModel model)
        {
            m_viewModel = model;
        }

        public bool SendChatMessage(ChatMessage message, string target)
        {
            if (m_personalCryptoManagers.ContainsKey(target))
            {

            }
            else
            {
                string keyBased = CryptoManager.GetAESKeyB64();
                string response = SendMessageAndWaitResponseCrypted(ServerSpecificStrings.SendSessionKeyToUser(m_userName, target, keyBased));
                if (response.Equals(ServerSpecificStrings.MesageOk))
                {
                    return StartNewChatSession(target, keyBased);
                }
            }
            return false;
        }

        void LoadStoredChat(string chatname, List<ChatMessage> messages)
        {
            //foreach(Message message in messages)
            //{
            //  ChatInstance.AddMessage(message);
            //}
            //UpadateView(ChatInstance);
        }

        bool StartNewChatSession(string username, string aesKey = "")
        {
            string keyBased = String.IsNullOrEmpty(aesKey) ? CryptoManager.GetAESKeyB64() : aesKey;

            CryptoManager cm = new CryptoManager(keyBased);
            if (m_personalCryptoManagers.ContainsKey(username))
            {
                m_personalCryptoManagers[username] = cm;
            }
            else
            {
                m_personalCryptoManagers.Add(username, cm);
            }
            return true;
        }
        void LoadClientData()
        {
            diskManager = new DiskManager("C:\\Work\\UserData.dat");
            m_viewModel.ChatTabs = ChatTabsToXmlConverter.ParseUserData(
                m_cryptoManager.DecryptUserDataB64(diskManager.LoadUserSettingsFromDisk(m_cryptoManager.GetUser())));
            foreach (ChatTab element in m_viewModel.ChatTabs)
            {
                if (element.chatType == ChatInstanceType.personal)
                {
                    m_viewModel.SetOnlineStatus(element.Name, StartNewChatSession(element.Name));
                }
            }
        }

        void RequestUnreadedMessages(string chatname, string password)
        {

        }

        void SaveClientData()
        {
            foreach (ChatTab tab in m_viewModel.ChatTabs)
            {
                if (tab.chatType == ChatInstanceType.group)
                {

                }
                if (tab.chatType == ChatInstanceType.personal)
                {

                }
            }
        }

        public void AddUserCredintals(string login, string password)
        {
            m_cryptoManager.SetUser(login, password);
        }

        bool ParseAndAddMessage(string message)
        {
            string sender = ServerSpecificStrings.ExtractParamValue("sender", message);
            string key = ServerSpecificStrings.ExtractParamValue("key", message);
            string chatType = ServerSpecificStrings.ExtractParamValue("ChatType", message);
            if (chatType.Contains("personal"))
            {
                if (!String.IsNullOrEmpty(key))
                {
                    StartNewChatSession(sender, key);
                    return true;
                }
                string text = m_personalCryptoManagers[sender].DecryptWithSessionB64(ServerSpecificStrings.ExtractParamValue("text", message));
                m_viewModel.AddMessage(sender, new ChatMessage(text, sender, DateTime.Now));
                return true;
            }

            if (chatType.Contains("group"))
            {
                foreach (ChatTab chatTab in m_viewModel.ChatTabs)
                {
                    if (chatTab.Name == sender)
                    {
                        string text = ServerSpecificStrings.ExtractParamValue("text", message);
                        string author = ServerSpecificStrings.ExtractParamValue("author", message);

                        m_viewModel.AddMessage(sender, new ChatMessage(text, author, DateTime.Now));
                        return true;
                    }
                }
            }
            return false;
        }

        void ServerMessagesThread()
        {
            while (threadStarted)
            {
                lock (_lockSendingWhileUpdate)
                {
                    try
                    {
                        string command = SendMessageAndWaitResponseCrypted("message=updateStatus");
                        if (!String.IsNullOrEmpty(command))
                        {
                            string[] delim = { ServerSpecificStrings.PackedMessageDelim };
                            string[] messages = command.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string message in messages)
                            {
                                ParseAndAddMessage(message);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

                Thread.Sleep(200);
            }
        }
    }
}
