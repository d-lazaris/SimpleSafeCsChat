using SimpleSafeCSChat.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimpleSafeCSChat.Test;

namespace SimpleSafeCSChat
{
    /// <summary>
    /// Interaction logic for ChatScreen.xaml
    /// </summary>
    public partial class ChatScreen : Window
    {
       
        public ChatScreen(ChatManager cm)
        {
            InitializeComponent();
            DataContext = new TestChatWindowViewModel(cm);
        }
    }
}
