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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BackgammonClient.ViewModels
{
    class ContactsViewModel : ViewModelPropertyChanged
    {
        public ICommand LogoutCommand { get; set; }
        public ICommand OpenChatCommad { get; set; }
        public ICommand OpenGameCommad { get; set; }
        public UserForView ChosenContact { get; set; }

        ClientUserManager _userManager;
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
        public ContactsViewModel()
        {
            _userManager = new ClientUserManager();
            ContactList = ConvertUserForUserView.ConvertUser(_userManager.GetContactList());
            _userManager.RegisterNotifyEvent(ContactUptaded);

            LogoutCommand = new RelayCommand(Logout);
            OpenChatCommad = new RelayCommand(OpenChat);
        }

        private void OpenChat()
        {
            if (ChosenContact != null)
            {
                ClientUserManager.UserToChatWith = ChosenContact.UserName;
                _userManager.ChangeUserStatus(UserState.busy);
                Application.Current.MainWindow.Content = new ChatPage();
            }
            else
            {
                MessageBox.Show("you need to choose user.");
            }
        }

        private void ContactUptaded(Dictionary<string, UserState> dictionary)
        {
            ContactList = ConvertUserForUserView.ConvertUser(dictionary);
        }

        private void Logout()
        {
            _userManager.InvokeLogout();
            Application.Current.MainWindow.Content = new RegisterPage();

        }
    }
}
