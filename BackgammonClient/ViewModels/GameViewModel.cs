using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BackgammonClient.ViewModels
{
    class GameViewModel : ViewModelPropertyChanged
    {
        //public ObservableCollection<StackPanel> c { get; set; }
        private ObservableCollection<Ellipse> _shapes;
        public ObservableCollection<Ellipse> Shapes
        {
            get { return _shapes; }
            set
            {
                _shapes = value;
                OnPropertyChanged();
            }
        }

        public List<StackPanel> cells;
        public GameViewModel()
        {
            StackPanel stackPanel = new StackPanel();
            //c.Add(stackPanel);
            Grid g = new Grid();
           
            Grid.(stackPanel, 2);
            for (int i = 0; i < 24; i++)
            {
            }
            Shapes = new ObservableCollection<Ellipse>();
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 30;
            ellipse.Height = 30;
            ellipse.Fill = new SolidColorBrush(Colors.Blue);
            Shapes.Add(ellipse);
        }
    }
}
