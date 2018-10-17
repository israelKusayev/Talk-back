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

namespace BackgammonClient.ViewModels
{
    class ChatViewModel : ViewModelPropertyChanged
    {
        public string UserToChatWith { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public string Message { get; set; }
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
        public ChatViewModel()
        {
            _userManager = new ClientUserManager();
            _chatManager = new ClientChatManager();
            UserToChatWith = ClientUserManager.UserToChatWith;
            SendMessageCommand = new RelayCommand(SandMessage);
            BackCommand = new RelayCommand(Back);
            _chatManager.RegisterSendMessageEvent(ReciveMessage);
        }

        private void Back()
        {
            _userManager.ChangeUserStatus(UserState.online);
            Application.Current.MainWindow.Content = new ContactPage();
        }

        private void SandMessage()
        {
            if (string.IsNullOrWhiteSpace(Message)) return;
            MessageBlock += ChatMessageFormatter.Format(Message, ClientUserManager.CurrentUser);
            _chatManager.InvokeSendMessage(Message);
        }

        private void ReciveMessage(string message, string senderName)

        {
            MessageBlock += ChatMessageFormatter.Format(message, senderName);
        }
    }
}
