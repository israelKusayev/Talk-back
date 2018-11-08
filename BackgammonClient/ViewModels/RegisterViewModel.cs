using BackgammonClient.BL;
using BackgammonClient.Helpers;
using BackgammonClient.Models;
using BackgammonClient.Utils;
using BackgammonClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BackgammonClient.ViewModels
{
    class RegisterViewModel
    {
        private IFrameNavigationService _navigationService;

        public User User { get; set; }
        private ClientUserManager _userManager;

        public ICommand RegisterCommand { get; set; }
        public ICommand LoginCommand { get; set; }

        public RegisterViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            User = new User();
            _userManager = new ClientUserManager();

            RegisterCommand = new RelayCommand(Register);
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(User.UserName) || string.IsNullOrWhiteSpace(User.Password))
            {
                MessageBox.Show("You must fill the username and password fields.");
            }
            else
            {
                if (_userManager.InvokeLogin(User))
                {
                    _navigationService.NavigateTo("Contacts");
                }
            }
        }

        private void Register()
        {
            if (String.IsNullOrWhiteSpace(User.UserName) || string.IsNullOrWhiteSpace(User.Password))
            {
                MessageBox.Show("You must fill the username and password fields.");
            }
            else
            {
                if (_userManager.InvokeRegister(User))
                {
                    _navigationService.NavigateTo("Contacts");
                }
            }
        }
    }
}
