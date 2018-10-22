using BackgammonClient.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BackgammonClient.ViewModels
{
    class GameViewModel : ViewModelPropertyChanged
    {
        //private readonly int[] _cellsOrganizer;
        private readonly int[,] _blackCheckers;
        private readonly int[,] _blueCheckers;
        public ICommand ClickMe { get; set; }
        private ObservableCollection<Ellipse>[] _cells;
        public ObservableCollection<Ellipse>[] Cells
        {
            get { return _cells; }
            set
            {
                _cells = value;
                OnPropertyChanged();
            }
        }
        public GameViewModel()
        {
            _blueCheckers = new int[,]
            {
                {0,5},
                {11,2},
                {17,5},
                {19,3}
            };

            _blackCheckers = new int[,]
            {
                {4,3},
                {6,5},
                {12,2},
                {23,5}
            };

            Cells = new ObservableCollection<Ellipse>[24];
            for (int i = 0; i < 24; i++)
            {
                Cells[i] = new ObservableCollection<Ellipse>();
            }

            InitializeCheckers();

        }

        private void InitializeCheckers()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < _blueCheckers[i, 1]; j++)
                {
                    Cells[_blueCheckers[i, 0]].Add(CreateChecker(true));
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < _blackCheckers[i, 1]; j++)
                {
                    Cells[_blackCheckers[i, 0]].Add(CreateChecker(false));
                }
            }
        }

        public Ellipse CreateChecker(bool isBlack)
        {
            Ellipse checker = new Ellipse
            {
                Width = 30,
                Height = 30
            };
            if (isBlack) checker.Fill = new SolidColorBrush(Colors.Black);
            else checker.Fill = new SolidColorBrush(Colors.Blue);
            return checker;
        }
    }
}
