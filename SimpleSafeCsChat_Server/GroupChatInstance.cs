using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server
{
    class GroupChatInstance
    {
        public readonly object _groupChatlocker = new object();
        public string chatName;
        public Dictionary<int, string> messages;
        public GroupChatInstance(string cn)
        {
            chatName = cn;
        }
    }
}
