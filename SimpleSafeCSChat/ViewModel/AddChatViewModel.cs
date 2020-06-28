using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCSChat.ViewModel
{
    public class AddChatViewModel : ViewModelBase
    {
        private string _chatName;
        public string ChatName 
        { 
            get
            {
                if (_chatName == null)
                {
                    _chatName = "";
                }
                return _chatName;
            }
            set
            {
                _chatName = value;
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                if (_password == null)
                {
                    _password = "";
                }
                return _password;
            }
            set
            {
                _password = value;
            }
        }
    }
}
