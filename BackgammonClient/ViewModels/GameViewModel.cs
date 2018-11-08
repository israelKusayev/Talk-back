using BackgammonClient.BL;
using BackgammonClient.Helpers;
using BackgammonClient.Models;
using BackgammonClient.Utils;
using General.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

        private IFrameNavigationService _navigationService;

        // private members.
        private ClientGameManager _gameManager;
        private bool _rollOnes;
        private int _selectedChecker = -1;
        private int _count;
        private Dice _dice;

        // Binding elements.
        private string _blackUserTitle;
        public string BlackUserTitle
        {
            get { return _blackUserTitle; }
            set
            {
                _blackUserTitle = value;
                OnPropertyChanged();
            }
        }

        private string _whiteUserTitle;
        public string WhiteUserTitle
        {
            get { return _whiteUserTitle; }
            set
            {
                _whiteUserTitle = value;
                OnPropertyChanged();
            }
        }


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

        private string _whiteBarredVisibility;
        public string WhiteBarredVisibility
        {
            get { return _whiteBarredVisibility; }
            set
            {
                _whiteBarredVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _blackBarredVisibility;
        public string BlackBarredVisibility
        {
            get { return _blackBarredVisibility; }
            set
            {
                _blackBarredVisibility = value;
                OnPropertyChanged();
            }
        }

        private int _whiteBarredCount;
        public int WhiteBarredCount
        {
            get { return _whiteBarredCount; }
            set
            {
                _whiteBarredCount = value;
                OnPropertyChanged();
            }
        }

        private int _blackBarredCount;
        public int BlackBarredCount
        {
            get { return _blackBarredCount; }
            set
            {
                _blackBarredCount = value;
                OnPropertyChanged();
            }
        }


        // ctor
        public GameViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _gameManager = new ClientGameManager();
            _gameManager.GetBoardState();

            InitializeBoard();
            InitializeBoardCheckers();
            CreateTitle();

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
            WhiteBarredVisibility = "Hidden";
            BlackBarredVisibility = "Hidden";

            if (ClientUserManager.CurrentUser == _gameManager._gameBoard.WhitePlayer)
            {
                Rotate = 180;
            }
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
            if (color == "black")
            {
                BlackUserTitle = $"Welcome {ClientUserManager.CurrentUser}  You play with {ClientUserManager.UserToChatWith}  You are the {color} player  {turnMessage}.";
            }
            else
            {
                WhiteUserTitle = $"Welcome {ClientUserManager.CurrentUser}  You play with {ClientUserManager.UserToChatWith}  You are the {color} player  {turnMessage}.";
            }
        }

        private void RollDice()
        {
            if (_gameManager._gameBoard.CurrentPlayer == ClientUserManager.CurrentUser)
            {
                if (!_rollOnes)
                {
                    _dice = _gameManager.RollDice();

                    StartTimer();

                    _rollOnes = true;
                }
            }
        }

        private void GetRivalDiceResult(Dice dice)
        {
            _dice = dice;
            StartTimer();
        }

        private void StartTimer()
        {
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 10;
            timer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Timer timer = sender as Timer;
            Random r = new Random();
            ImgCube1 = $"/Assets/Die_{r.Next(1, 7)}.jpg";
            ImgCube2 = $"/Assets/Die_{r.Next(1, 7)}.jpg";
            _count++;
            if (_count == 40)
            {
                _count = 0;
                timer.Stop();
                ImgCube1 = $"/Assets/Die_{_dice.Die1}.jpg";
                ImgCube2 = $"/Assets/Die_{_dice.Die2}.jpg";
            }
        }

        private void MoveChecker(string location)
        {
            if (_gameManager._gameBoard.CurrentPlayer == ClientUserManager.CurrentUser)// if is your turn
            {
                if (_rollOnes == false)
                {
                    MessageBox.Show("Please roll dice.");
                }

                if (MoveCheckerOut(location, _selectedChecker))
                {
                    return;
                }

                if (_selectedChecker == -1)
                {
                    //chack if user has Chacker in jail
                    if (_gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.BlackPlayer && _gameManager._gameBoard.BarredBlackCheckers != 0)
                    { }
                    else if (_gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.WhitePlayer && _gameManager._gameBoard.BarredWhiteCheckers != 0)
                    {
                        _selectedChecker = 24;
                    }
                    else
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
                        return;
                    }
                }

                int.TryParse(location, out int selectedLocation);
                if (selectedLocation != _selectedChecker)
                {
                    if (_gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.BlackPlayer && _gameManager._gameBoard.BarredBlackCheckers != 0 || _gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.WhitePlayer && _gameManager._gameBoard.BarredWhiteCheckers != 0)
                    {
                        if (!_gameManager.PrisonerCanEscape())
                        {
                            MessageBox.Show(Application.Current.MainWindow, "You can't ascape your turn changed.");
                        }
                    }
                    if (!_gameManager.MoveChecker(_selectedChecker, selectedLocation))
                    {
                        MessageBox.Show("cannot move checker to specified location");
                    } // else the hub will send the updated board to two players.
                }
                Mouse.OverrideCursor = Cursors.Arrow;
                _selectedChecker = -1;

            }
        }

        private bool MoveCheckerOut(string location, int selectedChacker)
        {
            if (selectedChacker != -1)
            {
                return false;
            }

            int.TryParse(location, out int from);
            if (!_gameManager.ValidateCheckerColor(from))
            {
                return false;
            }

            if (_gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.BlackPlayer && _gameManager._gameBoard.BlackCanTakeOut)
            {
                if (_gameManager._gameBoard.Dice.Die1 != -1 && _gameManager._gameBoard.Dice.Die1 >= 24 - from)//die =3 from = 24 - 21 
                {
                    _gameManager.MoveChecker(from, -2);
                    return true;
                }
                if (_gameManager._gameBoard.Dice.Die2 != -1 && _gameManager._gameBoard.Dice.Die2 >= 24 - from)//die =3 from = 24 - 21 
                {
                    _gameManager.MoveChecker(from, -2);
                    return true;
                }
            }
            else if (_gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.WhitePlayer && _gameManager._gameBoard.WhiteCanTakeOut)
            {
                if (_gameManager._gameBoard.Dice.Die1 != -1 && _gameManager._gameBoard.Dice.Die1 > from)//from = 3  die = 4 true
                {
                    _gameManager.MoveChecker(from, -2);
                    return true;
                }

                if (_gameManager._gameBoard.Dice.Die2 != -1 && _gameManager._gameBoard.Dice.Die2 > from)//from = 3  die = 4 true
                {
                    _gameManager.MoveChecker(from, -2);
                    return true;
                }
            }
            return false;
        }

        private void UpdateGameBoard()
        {
            CreateTitle();
            ChangeDiceVisibility();
            DisplayBarredCheckers();
            MoveCheckerOnBoard();
        }

        private void ChangeDiceVisibility()
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
        }

        private void DisplayBarredCheckers()
        {
            if (_gameManager._gameBoard.BarredBlackCheckers == 0)
            {
                BlackBarredVisibility = "Hidden";
            }
            else
            {
                BlackBarredVisibility = "Visible";
                BlackBarredCount = _gameManager._gameBoard.BarredBlackCheckers;
            }

            if (_gameManager._gameBoard.BarredWhiteCheckers == 0)
            {
                WhiteBarredVisibility = "Hidden";
            }
            else
            {
                WhiteBarredVisibility = "Visible";
                WhiteBarredCount = _gameManager._gameBoard.BarredWhiteCheckers;
            }
        }

        private void MoveCheckerOnBoard()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_gameManager._gameBoard.MoveTo == -2)
                {
                    _cells[_gameManager._gameBoard.MoveFrom].RemoveAt(0);
                    return;
                }
                if (_gameManager._gameBoard.IsBarred)
                {
                    _cells[_gameManager._gameBoard.MoveTo].RemoveAt(0);
                }

                if (_gameManager._gameBoard.MoveTo != -10)
                {
                    if (_gameManager._gameBoard.MoveFrom != 24 && _gameManager._gameBoard.MoveFrom != -1)
                    {
                        _cells[_gameManager._gameBoard.MoveFrom].RemoveAt(0);
                    }
                    bool color = _gameManager._gameBoard.CurrentPlayer == _gameManager._gameBoard.BlackPlayer ? true : false;
                    if (_gameManager._gameBoard.TurnChangaed)
                    {
                        _cells[_gameManager._gameBoard.MoveTo].Add(CreateChecker(!color));
                    }
                    else
                    {
                        _cells[_gameManager._gameBoard.MoveTo].Add(CreateChecker(color));
                    }
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
