using SimpleSafeCsChat_Server.SQLPresentable_;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server
{
    class ServerManager
    {
        //ServerData m_serverData;
        CryptoManager serverCryptoManager;
        Dictionary<string, UserInstance> m_connectedUsers;
        Dictionary<string, GroupChatInstance> m_groupChats;
        SQLManager sqlManager;
        Listener m_listener;
        public ServerManager(string password)
        {
            string serverUser = "Tester";
            string serverPassword = "Test1!";
            //"SCSC_SQLite.sqlite"
            SCSC_RSA sc = new SCSC_RSA();
            serverCryptoManager = new CryptoManager();
            serverCryptoManager.SetUser(serverUser, serverPassword);
            //TEST!!
            sqlManager = new SQLManager(serverUser, serverPassword);
            sqlManager.Start("C:\\Work\\Testdb.sqlite");
            sqlManager.CreateTables();
            AddUserToDb("Tester", "Tester");
            //TEST!!
            //m_serverData = new ServerData();
            //m_serverData.Load();
            m_connectedUsers = new Dictionary<string, UserInstance>();
            m_groupChats = new Dictionary<string, GroupChatInstance>();
            m_listener = new Listener(8989, this);
            m_listener.StartListen();
        }
        public string ProcessRequest(string reqest)
        {
            string user = TryGetUser(reqest);
            if (!String.IsNullOrEmpty(user))
            {
                UserInstance userInstance;
                m_connectedUsers.TryGetValue(user, out userInstance);
                string decryptedRequest = userInstance.cryptoManager.DecryptWithSessionB64(reqest);
                ServerCommands command = ParseCommand(decryptedRequest);
                switch (command)
                {
                    case ServerCommands.ClientUpdate:
                        return userInstance.cryptoManager.EncryptWithSessionB64(PackMessages(user));
                    case ServerCommands.ClientRegister:
                        return userInstance.cryptoManager.EncryptWithSessionB64(RegisterUser(decryptedRequest, user));
                    case ServerCommands.ClientAuth:
                        return userInstance.cryptoManager.EncryptWithSessionB64(AuthUser(decryptedRequest, user));
                    case ServerCommands.RecieveMessage:
                        return userInstance.cryptoManager.EncryptWithSessionB64(RetranslateMessage(decryptedRequest) ? ServerSpecificStrings.MesageOk : ServerSpecificStrings.MesageNotOnline);
                    case ServerCommands.AddGroupChat:
                        return userInstance.cryptoManager.EncryptWithSessionB64(AddGroupChat(decryptedRequest) ? ServerSpecificStrings.MesageOk : ServerSpecificStrings.MesageNotOnline);
                    case ServerCommands.OkCheck:
                        return userInstance.cryptoManager.EncryptWithSessionB64(ServerSpecificStrings.MesageOk);
                    default:
                        return "";
                }
            }
            else
            {
                // UserId is IV
                string newUser = TryAddUser(reqest);
                if (!String.IsNullOrEmpty(newUser))
                {
                    UserInstance userInstance;
                    m_connectedUsers.TryGetValue(newUser, out userInstance);
                    return userInstance.cryptoManager.EncryptWithSessionB64(ServerSpecificStrings.MesageOk);
                }

            }
            return "";
        }

        string PackMessages(string user)
        {
            string packedMessage = "message=packed&count={0}";
            if (m_connectedUsers.ContainsKey(user))
            {
                int count = 0;
                while (m_connectedUsers[user].userMessages.Count != 0)
                {
                    string message;
                    if (m_connectedUsers[user].userMessages.TryDequeue(out message))
                    {
                        packedMessage += "&pm=" + message;
                    }
                }
                packedMessage = String.Format(packedMessage, count);
            }

            return packedMessage;
        }
        void AddLastUnseenGroupMessages(string user, int number)
        {
            if (m_connectedUsers.ContainsKey(user))
            {
                lock (m_connectedUsers[user]._userLocker)
                {
                    for (int i = 0; i < m_connectedUsers[user].chatParticipate.Count; i++)
                    {
                        var item = m_connectedUsers[user].chatParticipate.ElementAt(i);
                        int modifiedSeen = item.Value + 1;
                        for (; modifiedSeen < modifiedSeen + number; modifiedSeen++)
                        {
                            var tryGetTargetOnline = m_connectedUsers.First(x => x.Value.userName == user);
                            m_connectedUsers[tryGetTargetOnline.Key].userMessages.Enqueue(m_groupChats[item.Key].messages[modifiedSeen]);
                        }
                        m_connectedUsers[user].chatParticipate[item.Key] = modifiedSeen;
                    }
                }
            }
        }
        ServerCommands ParseCommand(string reqest)
        {
            if (reqest.Contains(ServerSpecificStrings.MesageOk))
            {
                return ServerCommands.OkCheck;
            }
            if (reqest.Contains(ServerSpecificStrings.MesageTypeAuth))
            {
                return ServerCommands.ClientAuth;
            }
            if (reqest.Contains(ServerSpecificStrings.MesageTypeRegister))
            {
                return ServerCommands.ClientRegister;
            }
            if (reqest.Contains(ServerSpecificStrings.MesageTypeSend))
            {
                return ServerCommands.RecieveMessage;
            }
            if (reqest.Contains(ServerSpecificStrings.MesageTypeGroupEnter))
            {
                return ServerCommands.AddGroupChat;
            }
            if (reqest.Contains(ServerSpecificStrings.MesageTypeGroupExit))
            {
                return ServerCommands.OkCheck;
            }
            return ServerCommands.ClientAuth;
        }

        string TryGetUser(string reqest)
        {
            byte[] req = Convert.FromBase64String(reqest);

            string id = Convert.ToBase64String(req.Take(16).ToArray());
            if (m_connectedUsers.ContainsKey(id))
            {
                return id;
            }
            return "";
        }

        string TryAddUser(string request)
        {
            string decrypted = CryptoManager.DecryptRSAFromB64(request);
            if (decrypted.Contains("message"))
            {
                string[] delim = { "key=" };
                CryptoManager cm = new CryptoManager(decrypted.Split(delim, StringSplitOptions.RemoveEmptyEntries)[1]);
                string userId = Convert.ToBase64String(cm.m_sessionIV);
                //string userName = ServerSpecificStrings.ExtractParamValue("login", decrypted);
                m_connectedUsers.Add(userId, new UserInstance("", cm, new Dictionary<string, int>()));
                //m_usersCrypto.Add(userId, cm);
                //m_usersMessageQueue.Add(userId, new ConcurrentQueue<string>());
                return userId;
            }
            return "";
        }

        string RegisterUser(string request, string userId)
        {
            string userName = ServerSpecificStrings.ExtractParamValue("login", request);
            string password = ServerSpecificStrings.ExtractParamValue("password", request);
            if (!GetUserFromDb(userName))
            {
                AddUserToDb(userName, password);
                m_connectedUsers[userId].userName = userName;
                return ServerSpecificStrings.MesageOk;
            }
            return "";
        }

        string AuthUser(string request, string userId)
        {
            string userName = ServerSpecificStrings.ExtractParamValue("login", request);
            string password = ServerSpecificStrings.ExtractParamValue("password", request); ;
            password = password.Split('&')[0];
            if (GetUserFromDb(userName, password))
            {
                m_connectedUsers[userId].userName = userName;
                return ServerSpecificStrings.MesageOk;
            }
            return ServerSpecificStrings.MesageError;
        }

        bool RetranslateMessage(string request)
        {
            string type = ServerSpecificStrings.ExtractParamValue("ChatType", request);
            string target = ServerSpecificStrings.ExtractParamValue("target", request);
            if (type.Equals("Personal"))
            {
                try
                {
                    var tryGetTargetOnline = m_connectedUsers.First(x => x.Value.userName == target);
                    m_connectedUsers[tryGetTargetOnline.Key].userMessages.Enqueue(request);
                }
                catch (Exception e)
                {
                    return false;
                }

            }
            if (type.Equals("Group"))
            {
                AddMessageToGroupChat(target, request);
            }
            return true;
        }

        private readonly object _locker = new object();

        void AddMessageToGroupChat(string chatName, string message)
        {
            if (m_groupChats.ContainsKey(chatName))
            {
                lock (m_groupChats[chatName]._groupChatlocker)
                {
                    m_groupChats[chatName].messages.Add(m_groupChats[chatName].messages.Last().Key + 1, message);
                }
            }
        }

        bool AddGroupChat(string request)
        {
            string target = ServerSpecificStrings.ExtractParamValue("target", request);
            string password = ServerSpecificStrings.ExtractParamValue("password", request);
            if (!m_groupChats.ContainsKey(target))
            {
                CreateNewGroupChat(target, password);
            }
            string sender = ServerSpecificStrings.ExtractParamValue("sender", request);
            return TryAddUserToGroupChat(target, sender, password);
        }

        bool CreateNewGroupChat(string chatname, string password)
        {
            SQLPresentable_GroupChats groupChat = new SQLPresentable_GroupChats();
            groupChat.chatName = chatname;
            groupChat.password = serverCryptoManager.EncryptUserDataB64(password);
            sqlManager.WriteStructureToSql(groupChat);
            m_groupChats.Add(chatname, new GroupChatInstance(chatname));
            return true;
        }

        bool TryAddUserToGroupChat(string chatName, string userName, string password)
        {
            string chatId = CheckGroupChatPassword(chatName, password);
            if (!String.IsNullOrEmpty(chatId))
            {
                AddNewGroupChatParticipant(chatName, userName);
            }
            return false;
        }

        string CheckGroupChatPassword(string chatName, string password)
        {
            List<List<string>> structure = new List<List<string>> { new List<string> { "", "", "" } };
            string whereCondition = String.Format("groupChatName = '{0}'", chatName);
            sqlManager.ReadStructureFromSqlWhere(ref structure, "GroupChats", whereCondition);
            foreach (List<string> list in structure)
            {
                if (!String.IsNullOrEmpty(password))
                {
                    if (serverCryptoManager.EncryptUserDataB64(password) == list[2])
                    {
                        return list[0];
                    }

                }
            }
            return "";
        }

        bool AddNewGroupChatParticipant(string chatName, string userName)
        {

            return true;
        }

        bool GetUserFromDb(string userName, string password = "")
        {
            List<List<string>> structure = new List<List<string>> { new List<string> { "", "", "" } };
            string whereCondition = String.Format("username = '{0}'", userName);
            sqlManager.ReadStructureFromSqlWhere(ref structure, "users", whereCondition);
            foreach (List<string> list in structure)
            {
                if (list[1] == userName)
                {
                    if (!String.IsNullOrEmpty(password))
                    {
                        if (serverCryptoManager.EncryptUserDataB64(password) == list[2])
                        {
                            return true;
                        }

                    }

                }
            }
            return false;
        }

        void AddUserToDb(string userName, string password)
        {
            SQLPresentable_Users user = new SQLPresentable_Users();
            user.username = userName;
            user.password = password;
            sqlManager.WriteStructureToSql(user);
        }
    }
}
