using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCSChat.ViewModel
{
    public enum ChatInstanceType
    {
        personal = 0,
        group
    }
    public class ChatTab : ViewModelBase
    {
        public ChatInstanceType chatType;

        public string Password;
        public string Name { get; set; }
        public ObservableCollection<ChatMessage> ChatMessages { get; set; }

        ChatMessage m_newMessage;
        public ChatMessage NewMessage
        {
            get
            {
                if (m_newMessage == null)
                {
                    m_newMessage = new ChatMessage();
                }
                return m_newMessage;
            }
            set { OnPropertyChanged(ref m_newMessage, value); }
        }

        private bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set {OnPropertyChanged(ref _isLoggedIn, value); }
        }

        private bool _hasSentNewMessage;
        public bool HasSentNewMessage
        {
            get { return _hasSentNewMessage; }
            set { OnPropertyChanged(ref _hasSentNewMessage, value); }
        }
        public ChatTab() 
        { 
            ChatMessages = new ObservableCollection<ChatMessage>(); 
        }
        public ChatTab(string name)
        {
            Name = name;
            ChatMessages = new ObservableCollection<ChatMessage>();
        }
    }
}
