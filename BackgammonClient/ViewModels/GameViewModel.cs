using BackgammonClient.BL;
using BackgammonClient.Models;
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
        GameBoardState gameBoard;

        // ctor
        public GameViewModel()
        {
            Cells = new ObservableCollection<Ellipse>[24];
            for (int i = 0; i < 24; i++)
            {
                Cells[i] = new ObservableCollection<Ellipse>();
            }
            gameBoard = _gameManager.GetBoardState();


            for (int i = 1; i < 25; i++)
            {
                if (gameBoard.WhiteCheckersLocation.ContainsKey(i))
                {
                    for (int j = 0; j < gameBoard.WhiteCheckersLocation[i]; j++)
                    {
                        _cells[i].Add(CreateChecker(false));
                    }
                }
            }

            for (int i = 1; i < 25; i++)
            {
                if (gameBoard.BlackCheckersLocation.ContainsKey(i))
                {
                    for (int j = 0; j < gameBoard.BlackCheckersLocation[i]; j++)
                    {
                        _cells[i].Add(CreateChecker(false));
                    }
                }
            }

            //InitializeCheckers();

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
