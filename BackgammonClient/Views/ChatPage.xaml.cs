using BackgammonClient.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackgammonClient.Views
{
    /// <summary>
    /// Interaction logic for ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        //private static ChatPage _page;

        //public static ChatPage Pagee
        //{
        //    get
        //    {
        //        if (_page == null)
        //        {
        //            _page = new ChatPage();
        //        }
        //        return _page;
        //    }
        //}


        public ChatPage()
        {
            InitializeComponent();
            DataContext = new ChatViewModel();

        }
    }
}
