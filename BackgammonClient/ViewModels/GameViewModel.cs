using BackgammonClient.BL;
using BackgammonClient.Utils;
using General.Models;
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
        private ClientGameManager _gameManager;

        private readonly int[,] _blackCheckers;
        private readonly int[,] _blueCheckers;
        private readonly string[] _DiceImgPath;
        public ICommand RollDiceCommand { get; set; }
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

        private string _imgCube1;
        public string ImgCube1
        {
            get { return _imgCube1; }
            set
            {
                _imgCube1 = value;
                OnPropertyChanged();
            }
        }

        private string _imgCube2;
        public string ImgCube2
        {
            get { return _imgCube2; }
            set
            {
                _imgCube2 = value;
                OnPropertyChanged();
            }
        }


        // ctor
        public GameViewModel()
        {
            _blackCheckers = new int[,]
            {
                {0,2},
                {11,5},
                {16,3},
                {18,5}
            };

            _blueCheckers = new int[,]
            {
                {5,5},
                {7,3},
                {12,5},
                {23,2}
            };
            //_DiceImgPath = new string[6] { "/Assets/Die_1.jpg", "/Assets/Die_2.jpg", "/Assets/Die_3.jpg", "/Assets/Die_4.jpg", "/Assets/Die_5.jpg", "/Assets/Die_6.jpg" };
            Cells = new ObservableCollection<Ellipse>[24];
            for (int i = 0; i < 24; i++)
            {
                Cells[i] = new ObservableCollection<Ellipse>();
            }

            InitializeCheckers();

            RollDiceCommand = new RelayCommand(RollDice);
            _gameManager = new ClientGameManager();
            _gameManager.RegisterGetDiceEvent(GetRivalDiceResult);
        }

        private void RollDice()
        {
            Dice dice = _gameManager.RollDice();
            ImgCube1 = $"/Assets/Die_{dice.Die1}.jpg";
            ImgCube2 = $"/Assets/Die_{dice.Die2}.jpg";
        }

        private void GetRivalDiceResult(Dice dice)
        {
            ImgCube1 = $"/Assets/Die_{dice.Die1}.jpg";
            ImgCube2 = $"/Assets/Die_{dice.Die2}.jpg";
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
