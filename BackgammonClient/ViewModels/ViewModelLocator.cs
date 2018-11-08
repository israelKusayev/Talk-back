using BackgammonClient.Helpers;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.ViewModels
{
    class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
            SimpleIoc.Default.Register<ContactsViewModel>();
            SimpleIoc.Default.Register<ChatViewModel>();
            SimpleIoc.Default.Register<GameViewModel>();
            SetupNavigation();
        }

        private static void SetupNavigation()
        {
            var navigationService = new FrameNavigationService();
            navigationService.Configure("Register", new Uri("../Views/RegisterPage.xaml", UriKind.Relative));
            navigationService.Configure("Contacts", new Uri("../Views/ContactPage.xaml", UriKind.Relative));
            navigationService.Configure("Chat", new Uri("../Views/ChatPage.xaml", UriKind.Relative));
            navigationService.Configure("Game", new Uri("../Views/GamePage.xaml", UriKind.Relative));
            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public RegisterViewModel RegisterViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RegisterViewModel>();
            }
        }
        public ContactsViewModel ContactsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ContactsViewModel>();
            }
        }
        public ChatViewModel ChatViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChatViewModel>();
            }
        }
        public GameViewModel GameViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GameViewModel>();
            }
        }
        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}
