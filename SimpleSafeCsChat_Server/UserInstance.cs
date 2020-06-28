using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server
{
    class UserInstance
    {
        public readonly object _userLocker = new object();
        public string userName;
        public CryptoManager cryptoManager;
        public Dictionary<string, int> chatParticipate;
        public ConcurrentQueue<string> userMessages;
        public UserInstance(string un, CryptoManager cm, Dictionary<string, int> cp)
        {
            userName = un;
            cryptoManager = cm;
            chatParticipate = cp;
            userMessages = new ConcurrentQueue<string>();
        }
    }
}
