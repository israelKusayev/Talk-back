using BackgammonClient.BL;
using BackgammonClient.Converters;
using BackgammonClient.Models;
using BackgammonClient.Utils;
using BackgammonClient.Views;
using General.Emuns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace BackgammonClient.ViewModels
{
    class ContactsViewModel : ViewModelPropertyChanged
    {
        private bool isChat;

        public ICommand LogoutCommand { get; set; }
        public ICommand OpenChatCommand { get; set; }
        public ICommand OpenGameCommand { get; set; }
        public UserForView ChosenContact { get; set; }

        ClientUserManager _userManager;
        ClientChatManager _chatManager;
        private ObservableCollection<UserForView> _contactList;
        public ObservableCollection<UserForView> ContactList
        {
            get
            {
                return _contactList;
            }
            set
            {
                _contactList = value;
                OnPropertyChanged();
            }
        }

        //ctor
        public ContactsViewModel()
        {
            _userManager = new ClientUserManager();
            _chatManager = new ClientChatManager();
            ContactList = ConvertUserForUserView.ConvertUser(_userManager.GetContactList());
            _userManager.RegisterNotifyEvent(ContactUptaded);
            _chatManager.RegisterInvitationResponseEvent(HandleUserResponse);
            _chatManager.RegisterChatRequestEvent(AgreeChatRequest);

            LogoutCommand = new RelayCommand(Logout);
            OpenChatCommand = new RelayCommand(OpenChat);
            OpenGameCommand = new RelayCommand(OpenGame);
        }



        // update contacts list.
        private void ContactUptaded(Dictionary<string, UserState> dictionary)
        {
            ContactList = ConvertUserForUserView.ConvertUser(dictionary);
        }

        //Sand request to another user to chat with him.
        private void Open(bool isChat)
        {
            if (ChosenContact != null)
            {
                if (ChosenContact.State == UserState.offline)
                {
                    MessageBox.Show("User is offline.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (ChosenContact.State == UserState.busy)
                {
                    MessageBox.Show("User is not available.");
                }
                else
                {
                    ClientUserManager.UserToChatWith = ChosenContact.UserName;
                    _userManager.ChangeUserStatus(UserState.busy);
                    _chatManager.SendRequest(isChat);
                }
            }
            else
            {
                MessageBox.Show("you need to choose user.");
            }
        }

        //For reciver, after he agree to chat request.
        private void AgreeChatRequest(bool isChat)
        {

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isChat) Application.Current.MainWindow.Content = new ChatPage();
                else Application.Current.MainWindow.Content = new GamePage();

            }));

        }

        //For sender, after reciver agree to chat request.
        private void HandleUserResponse(bool response)
        {
            if (response)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (isChat) Application.Current.MainWindow.Content = new ChatPage();
                    else Application.Current.MainWindow.Content = new GamePage();
                }));
            }
            else
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show("user refused to join your chat.");
                }));
            }
        }

        private void OpenChat()
        {
            isChat = true;
            Open(isChat);
        }

        private void OpenGame()
        {
            isChat = false;
            Open(false);
        }

        //Logout to register page.
        private void Logout()
        {
            if (_userManager.InvokeLogout())
            {
                Application.Current.MainWindow.Content = new RegisterPage();
            }
        }
    }
}
