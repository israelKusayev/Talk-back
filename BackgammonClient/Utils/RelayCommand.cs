using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackgammonClient.Utils
{
    public class RelayCommand : ICommand
    {
        readonly Action _act;
        public RelayCommand(Action act)
        {
            _act = act;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _act();//1
        }
    }
}
