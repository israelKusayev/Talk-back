using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackgammonClient.Utils
{
    class RelayCommandWithParams<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action<T> _act;
        public RelayCommandWithParams(Action<T> act)
        {
            _act = act;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _act((T)parameter);
        }
    }
}
