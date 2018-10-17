using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.ViewModels
{
    class ViewModelPropertyChanged : INotifyPropertyChanged
    {

        //public event PropertyChangedEventHandler OnViewChange;
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //protected virtual void ViewChanging([CallerMemberName] string propertyName = null)
        //{
        //    OnViewChange?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}



