using BackgammonClient.BL;
using BackgammonClient.Models;
using BackgammonClient.Utils;
using BackgammonClient.Views;
using General.Emuns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace BackgammonClient.ViewModels
{
    class ChatViewModel : ViewModelPropertyChanged
    {
        public string UserToChatWith { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand BackCommand { get; set; }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _messageBlock;
        public string MessageBlock
        {
            get { return _messageBlock; }
            set
            {
                _messageBlock = value;
                OnPropertyChanged();
            }
        }

        private ClientUserManager _userManager;
        private ClientChatManager _chatManager;

        public string UserTitle { get; set; }

        //ctor
        public ChatViewModel()
        {
            _userManager = ClientUserManager.Instance;
            _chatManager = ClientChatManager.Instance;
            UserToChatWith = $"You are talking with: { ClientUserManager.UserToChatWith}";
            UserTitle = $"Welcome {ClientUserManager.CurrentUser}";
            SendMessageCommand = new RelayCommand(SandMessage);
            BackCommand = new RelayCommand(Back);
            _chatManager.RegisterSendMessageEvent(ReciveMessage);
            _chatManager.RegisterUserDisconnectedEvent(ReturnToContactPage);
        }

        // Send message to user.
        private void SandMessage()
        {

            if (string.IsNullOrWhiteSpace(Message)) return;
            MessageBlock += ChatMessageFormatter.Format(Message, ClientUserManager.CurrentUser);
            _chatManager.InvokeSendMessage(Message);
            Message = "";
        }

        // Recive message from user.
        private void ReciveMessage(string message, string senderName)
        {
            MessageBlock += ChatMessageFormatter.Format(message, senderName);
        }

        // Back to contacts page
        private void Back()
        {
            _userManager.ChangeUserStatus(UserState.online);

            _chatManager.UserDisconnected();
            Application.Current.MainWindow.Content = new ContactPage();
        }


        // this function called from the server, to close the chat to second user.
        private void ReturnToContactPage()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(Application.Current.MainWindow, "User leave the chat");
                Application.Current.MainWindow.Content = new ContactPage();
            }));
        }

    }
}
