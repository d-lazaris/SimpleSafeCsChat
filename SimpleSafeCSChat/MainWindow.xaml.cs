using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleSafeCSChat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnRegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnLoginButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bgWrkr = new BackgroundWorker();
            bgWrkr.DoWork += new DoWorkEventHandler(bgWrkr_DoWork);
            bgWrkr.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWrkr_RunWorkerCompleted);
            BlockAllElements();
            bgWrkr.RunWorkerAsync();
        }

        private void bgWrkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            ChatManager result = (ChatManager)e.Result;
            if (result != null)
            {
                ChatScreen chatScreen = new ChatScreen(result);
                this.Hide();
                chatScreen.ShowDialog();
                this.Show();

            }
            else
            {
                MainWindow_MessagesLabel.Content = "Failed to auth!";
                MainWindow_MessagesLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
            UnblockAllElements();
        }

        private void bgWrkr_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            object tbToFound = null;
            object tblToFound = null;
            object tbpToFound = null;
            this.Dispatcher.Invoke(new Action(()=> { 
                tbToFound = FindName("ServerAddressTextBox");
                tblToFound = FindName("LoginTexBox");
                tbpToFound = FindName("PasswordBox");
            }));
            if (tbToFound is TextBox && tblToFound is TextBox && tbpToFound is PasswordBox)
            {
                TextBox serverAddresTextBox = tbToFound as TextBox;
                TextBox loginTexBox = tblToFound as TextBox;
                PasswordBox passwordBox = tbpToFound as PasswordBox;
                string[] serverAddress = null;
                string login = "";
                string password = "";
                this.Dispatcher.Invoke(new Action(() => {
                    serverAddress = serverAddresTextBox.Text.Split(':');
                    login = loginTexBox.Text;
                    password = passwordBox.Password;
                }));
                if(Networker.TryConnect(serverAddress[0], Convert.ToInt32(serverAddress[1])))
                {
                    chatManagerError cmError;
                    ChatManager cm = ServerConnectionHelper.TryAuth(
                        new Networker(serverAddress[0], Convert.ToInt32(serverAddress[1])),
                        login,
                        password,
                        out cmError);
                    if (cm != null)
                    {
                        e.Result = cm;
                    }
                };
            }

        }

        private void BlockAllElements()
        {
            object gridToFound = FindName("MainWindowGrid");
            if (gridToFound is Grid)
            {
                Grid mwGrid = gridToFound as Grid;
                mwGrid.IsEnabled = false;
            }
        }

        private void UnblockAllElements()
        {
            object gridToFound = FindName("MainWindowGrid");
            if (gridToFound is Grid)
            {
                Grid mwGrid = gridToFound as Grid;
                mwGrid.IsEnabled = true;
            }
        }
    }
}
