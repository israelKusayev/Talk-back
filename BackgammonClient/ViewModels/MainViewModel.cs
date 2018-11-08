using BackgammonClient.Helpers;
using BackgammonClient.Utils;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IFrameNavigationService _navigationService;

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand(
                    () =>
                    {
                        _navigationService.NavigateTo("Register");
                    }));
            }
        }

        public MainViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
