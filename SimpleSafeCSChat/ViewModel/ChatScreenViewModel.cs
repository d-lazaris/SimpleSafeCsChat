using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SimpleSafeCSChat.ViewModel
{
    public class ChatScreenViewModel : ViewModelBase
    {
        protected ChatManager chatManager;

        private ObservableCollection<ChatTab> _chatTabs = new ObservableCollection<ChatTab>();
        public ObservableCollection<ChatTab> ChatTabs
        {
            get { return _chatTabs; }
            set
            {
                OnPropertyChanged(ref _chatTabs, value);
            }
        }

        private ChatTab m_selectedChatTab;

        public ChatTab SelectedChatTab
        {
            get
            {
                return m_selectedChatTab;
            }
            set
            {
                OnPropertyChanged(ref m_selectedChatTab, value);
            }
        }

        private ICommand _sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand ?? (_sendMessageCommand =
                    new Command(obj => SendMessage())
                    );
            }
        }

        private bool SendMessage()
        {
            try
            {
                string recepient = m_selectedChatTab.Name;
                SelectedChatTab.NewMessage.Time = DateTime.Now;
                if (chatManager.SendChatMessage(SelectedChatTab.NewMessage, recepient))
                {
                    SelectedChatTab.ChatMessages.Add(SelectedChatTab.NewMessage);
                    SelectedChatTab.NewMessage = new ChatMessage();
                    return true;
                }
                else
                {
                    MessageBox.Show("Error: User not online, try again later");
                    return false;
                }
                
            }
            catch (Exception e) 
            {
                MessageBox.Show("Error: unknown error");
                SelectedChatTab.NewMessage = new ChatMessage();
                return false; 
            }
        }

        private bool CanSendMessage()
        {
            return true;
        }

        private ICommand _addChatCommand;
        public ICommand AddChatCommand
        {
            get
            {
                return _addChatCommand ?? (_addChatCommand =
                    new Command(obj => AddChat())
                    );
            }
        }

        private bool AddChat()
        {
            AddChatViewModel acvm = new AddChatViewModel();
            AddChatWindow acw = new AddChatWindow(acvm);
            if (acw.ShowDialog() == true)
            {
                if (chatManager.AddChat(acvm.ChatName, acvm.Password))
                {
                    ChatTab chatTab = new ChatTab(acvm.ChatName);
                    ChatTabs.Add(chatTab);
                    return true;
                }
            }
            return false;
        }

        private readonly object _locker = new object();
        public bool AddMessage(string chatTabName, ChatMessage message)
        {
            lock(_locker)
            {
                var existing = ChatTabs.Where(c => c.Name == chatTabName);
                if (existing.Count() == 0)
                {
                    ChatTab chatTab = new ChatTab(chatTabName);
                    chatTab.ChatMessages.Add(message);
                    ChatTabs.Add(chatTab);
                }
                else
                {
                    existing.Select(c => { c.ChatMessages.Add(message); return c; }).ToList();
                }
                
            }
            return true;
        }

        public bool SetOnlineStatus(string chatTabName, bool status)
        {
            _chatTabs.Where(c => c.Name == chatTabName).Select(c => { c.IsLoggedIn = status; return c; }).ToList();
            return true;
        }

        public ChatScreenViewModel(ChatManager cm)
        {
            chatManager = cm;
        }
        public ChatScreenViewModel()
        {
            chatManager = null;
        }
    }
}
