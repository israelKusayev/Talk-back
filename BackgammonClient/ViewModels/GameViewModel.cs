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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BackgammonClient.ViewModels
{
    class GameViewModel : ViewModelPropertyChanged
    {
        // Commands.
        public ICommand RollDiceCommand { get; set; }
        public ICommand ChooseCheckerCommand { get; set; }

        // private members.
        private ClientGameManager _gameManager;
        private bool _rollOnes;
        private int _selectedChecker = -1;

        // Binding elements.
        public string UserTitle { get; set; }

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

        public int Rotate { get; set; }

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

        private string _diceVisibilityGroup1;
        public string DiceVisibilityGroup1
        {
            get { return _diceVisibilityGroup1; }
            set
            {
                _diceVisibilityGroup1 = value;
                OnPropertyChanged();
            }
        }

        private string _diceVisibilityGroup2;
        public string DiceVisibilityGroup2
        {
            get { return _diceVisibilityGroup2; }
            set
            {
                _diceVisibilityGroup2 = value;
                OnPropertyChanged();
            }
        }


        // ctor
        public GameViewModel()
        {
            _gameManager = new ClientGameManager();
            InitializeBoard();
            _gameManager.GetBoardState();
            InitializeBoardCheckers();
            CreateTitle();

            if (ClientUserManager.CurrentUser == _gameManager._gameBoard.WhitePlayer)
            {
                Rotate = 180;
            }


            RollDiceCommand = new RelayCommand(RollDice);
            ChooseCheckerCommand = new RelayCommandWithParams<string>(MoveChecker);
            _gameManager.RegisterGetDiceEvent(GetRivalDiceResult);
            _gameManager.RegisterBoardUpdatedEvent(UpdateGameBoard);
        }

        private void InitializeBoard()
        {
            Cells = new ObservableCollection<Ellipse>[24];
            for (int i = 0; i < 24; i++)
            {
                Cells[i] = new ObservableCollection<Ellipse>();
            }

            DiceVisibilityGroup1 = "Visible";
            DiceVisibilityGroup2 = "Hidden";
        }

        private void InitializeBoardCheckers()
        {
            for (int i = 0; i < 24; i++)
            {
                if (_gameManager._gameBoard.WhiteCheckersLocation.ContainsKey(i))
                {
                    for (int j = 0; j < _gameManager._gameBoard.WhiteCheckersLocation[i]; j++)
                    {
                        _cells[i].Add(CreateChecker(false));
                    }
                }
            }
            for (int i = 0; i < 24; i++)
            {
                if (_gameManager._gameBoard.BlackCheckersLocation.ContainsKey(i))
                {
                    for (int j = 0; j < _gameManager._gameBoard.BlackCheckersLocation[i]; j++)
                    {
                        _cells[i].Add(CreateChecker(true));
                    }
                }
            }
        }

        private void CreateTitle()
        {
            string color = ClientUserManager.CurrentUser == _gameManager._gameBoard.BlackPlayer ? "black" : "blue";
            string turnMessage = ClientUserManager.CurrentUser == _gameManager._gameBoard.CurrentPlayer ? "It is your turn" : "It is your rival turn";
            UserTitle = $"Welcome {ClientUserManager.CurrentUser}  You play with {ClientUserManager.UserToChatWith}  You are the {color} player  {turnMessage}.";
        }

        private void RollDice()
        {
            if (_gameManager._gameBoard.CurrentPlayer == ClientUserManager.CurrentUser)
            {
                if (!_rollOnes)
                {
                    Dice dice = _gameManager.RollDice();
                    ImgCube1 = $"/Assets/Die_{dice.Die1}.jpg";
                    ImgCube2 = $"/Assets/Die_{dice.Die2}.jpg";
                    _rollOnes = true;
                }
            }
        }

        private void GetRivalDiceResult(Dice dice)
        {
            ImgCube1 = $"/Assets/Die_{dice.Die1}.jpg";
            ImgCube2 = $"/Assets/Die_{dice.Die2}.jpg";
        }

        private void MoveChecker(string location)
        {
            if (_gameManager._gameBoard.CurrentPlayer == ClientUserManager.CurrentUser)// if is your turn
            {
                if (_selectedChecker == -1)
                {
                    int.TryParse(location, out _selectedChecker);
                    if (!_gameManager.ValidateCheckerColor(_selectedChecker))
                    {
                        _selectedChecker = -1; // That means he did not click on his color.
                    }
                    else
                    {
                        Mouse.OverrideCursor = Cursors.Hand;
                    }
                }
                else
                {
                    int.TryParse(location, out int selectedLocation);
                    if (selectedLocation != _selectedChecker)
                    {
                        if (!_gameManager.MoveChecker(_selectedChecker, selectedLocation))
                        {
                            MessageBox.Show("cannot move checker to specified location");
                        } // else the hub will send the updated board to two players.
                    }
                    Mouse.OverrideCursor = Cursors.Arrow;
                    _selectedChecker = -1;
                }
            }
        }

        private void UpdateGameBoard()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_gameManager._gameBoard.TurnChangaed)
                {
                    if (_gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.BlackPlayer)
                    {

                        DiceVisibilityGroup1 = "Hidden";
                        DiceVisibilityGroup2 = "Visible";
                    }
                    else
                    {
                        DiceVisibilityGroup1 = "Visible";
                        DiceVisibilityGroup2 = "Hidden";
                    }
                    _rollOnes = false;
                }


                _cells[_gameManager._gameBoard.MoveFrom].RemoveAt(0);
                bool color = _gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.BlackPlayer ? true : false;
                if (_gameManager._gameBoard.TurnChangaed)
                {
                    _cells[_gameManager._gameBoard.MoveTo].Add(CreateChecker(!color));
                }
                else
                {
                    _cells[_gameManager._gameBoard.MoveTo].Add(CreateChecker(color));
                }
            }));
        }

        private Ellipse CreateChecker(bool isBlack)
        {
            Ellipse checker = new Ellipse
            {
                Width = 30,
                Height = 30
            };
            if (isBlack) checker.Fill = new SolidColorBrush(Colors.Black);
            else checker.Fill = new SolidColorBrush(Colors.Blue);//white is the first plyer.
            return checker;
        }
    }
}
