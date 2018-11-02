using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BackgammonServer.Models;
using General.Interfaces;
using General.Models;

namespace BackgammonServer.BL
{
    public class ServerGameManager
    {
        internal Dictionary<string, GameBoardState> _boards;
        //internal string currentTurn;//?

        // Page instance
        private static ServerGameManager _instance;
        public static ServerGameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServerGameManager();

                }
                return _instance;
            }
            set { _instance = value; }
        }

        //ctor
        private ServerGameManager()
        {
            _boards = new Dictionary<string, GameBoardState>();
        }

        internal void InitializeBoard(string senderName, string reciverName)
        {
            string guid = senderName;
            GameBoardState gameBoard = new GameBoardState(senderName, reciverName);
            _boards.Add(guid, gameBoard);

        }

        internal string GetGameKey(string senderName, string reciverName)
        {
            return _boards.ContainsKey(senderName) ? senderName : reciverName;
        }

        internal IGameBoardState GetGameBoard(string gameKey)
        {
            return _boards[gameKey];
        }

        internal Dice RollDice(string gameKey)
        {
            Random rand = new Random();
            Dice dice = new Dice() { Die1 = rand.Next(1, 7), Die2 = rand.Next(1, 7) };
            _boards[gameKey].Dice = dice;
            if (dice.Die1 == dice.Die2)
            {
                _boards[gameKey].IsDouble = true;
            }
            else
            {
                _boards[gameKey].IsDouble = false;
            }
            return dice;
        }

        //black is asc// white is desc;

        internal bool MoveChecker(int from, int to, string gameKey)
        {

            GameBoardState temp = _boards[gameKey];
            temp.IsBarred = false;
            bool isBlack = temp.CurrentPlayer == temp.BlackPlayer;

            if (isBlack)// if is the black player.
            {
                if (!PrisonerCanEscape(gameKey, temp))
                {
                    temp.CurrentPlayer = temp.WhitePlayer;
                    temp.TurnChangaed = true;
                    return true;
                }

                if (temp.BarredBlackCheckers != 0 && from != -1)
                {
                    return false;
                }



                if (to <= from || !CorrectStep(temp, Math.Abs(from - to)))// if he want to move checker on rival checker.
                {
                    return false;
                }
                if (temp.WhiteCheckersLocation.ContainsKey(to))
                {
                    if (temp.WhiteCheckersLocation[to] == 1)
                    {
                        SendCheckerToJail(temp, isBlack, to);
                    }
                    else
                    {
                        return false;
                    }
                }
                MoveCheckerInDictionary(to, temp.BlackCheckersLocation);

                if (temp.BarredBlackCheckers == 0)
                {
                    RemoveCheckerFromDictionary(from, temp.BlackCheckersLocation);
                }
                else
                {
                    temp.BarredBlackCheckers--;
                }

                ReplaceTurn(temp, temp.WhitePlayer);

            }
            else // if is the white player.
            {
                if (!PrisonerCanEscape(gameKey, temp))
                {
                    temp.CurrentPlayer = temp.BlackPlayer;
                    temp.TurnChangaed = true;
                    return true;
                }

                if (temp.BarredWhiteCheckers != 0 && from != 24)
                {
                    return false;//?
                }
                if (from <= to || !CorrectStep(temp, Math.Abs(from - to)))
                {
                    return false;
                }

                if (temp.BlackCheckersLocation.ContainsKey(to))
                {
                    if (temp.BlackCheckersLocation[to] == 1)
                    {
                        SendCheckerToJail(temp, isBlack, to);
                    }
                    else
                    {
                        return false;
                    }
                }

                MoveCheckerInDictionary(to, temp.WhiteCheckersLocation);

                if (temp.BarredWhiteCheckers == 0)
                {
                    RemoveCheckerFromDictionary(from, temp.WhiteCheckersLocation);
                }
                else
                {
                    temp.BarredWhiteCheckers--;
                }
                ReplaceTurn(temp, temp.BlackPlayer);
            }
            temp.MoveFrom = from;
            temp.MoveTo = to;
            _boards[gameKey] = temp;
            return true;
        }


        internal bool PrisonerCanEscape(string gameKey, GameBoardState board = null)
        {
            GameBoardState gameBoard = board;
            if (gameBoard == null)
            {
                gameBoard = _boards[gameKey];
            }

            bool isBlack = gameBoard.CurrentPlayer == gameBoard.BlackPlayer;

            if (isBlack)
            {
                if (gameBoard.BarredBlackCheckers != 0)
                {
                    if (gameBoard.WhiteCheckersLocation.ContainsKey(gameBoard.Dice.Die1 - 1) && gameBoard.WhiteCheckersLocation.ContainsKey(gameBoard.Dice.Die2 - 1))
                    {
                        if (gameBoard.WhiteCheckersLocation[gameBoard.Dice.Die1 - 1] > 1 && gameBoard.WhiteCheckersLocation[gameBoard.Dice.Die2 - 1] > 1)
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (gameBoard.BarredWhiteCheckers != 0)
                {
                    if (gameBoard.BlackCheckersLocation.ContainsKey(24 - gameBoard.Dice.Die1) && gameBoard.BlackCheckersLocation.ContainsKey(24 - gameBoard.Dice.Die2))
                    {
                        if (gameBoard.BlackCheckersLocation[24 - gameBoard.Dice.Die1] > 1 && gameBoard.BlackCheckersLocation[24 - gameBoard.Dice.Die2] > 1)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;

        }

        private void SendCheckerToJail(GameBoardState tempBoard, bool isBlack, int to)
        {
            if (isBlack)
            {
                RemoveCheckerFromDictionary(to, tempBoard.WhiteCheckersLocation);
                tempBoard.BarredWhiteCheckers++;
            }
            else
            {
                RemoveCheckerFromDictionary(to, tempBoard.BlackCheckersLocation);
                tempBoard.BarredBlackCheckers++;
            }
            tempBoard.IsBarred = true;
        }

        private bool CorrectStep(GameBoardState temp, int steps)
        {
            if (temp.IsDouble)
            {
                if (temp.Dice.Die1 == steps)
                {
                    return true;
                }
                else
                {
                    return false;
                }//?
            }
            if (temp.Dice.Die1 == steps)
            {
                temp.Dice.Die1 = -1;
                return true;
            }
            if (temp.Dice.Die2 == steps)
            {
                temp.Dice.Die2 = -1;
                return true;
            }
            return false;
        }

        private void ReplaceTurn(GameBoardState temp, string newTurn)
        {
            if (temp.IsDouble)
            {
                if (temp._countMovments == 3)
                {
                    temp.CurrentPlayer = newTurn;
                    temp._countMovments = 0;
                    temp.TurnChangaed = true;
                }
                else
                {
                    temp._countMovments++;
                    temp.TurnChangaed = false;
                }
                return;
            }
            if (temp.Dice.Die1 == -1 && temp.Dice.Die2 == -1)
            {
                temp.CurrentPlayer = newTurn;
                temp.TurnChangaed = true;
            }
            else
            {
                temp.TurnChangaed = false;
            }
        }

        private void MoveCheckerInDictionary(int to, Dictionary<int, int> checkersDictionary)
        {
            if (checkersDictionary.ContainsKey(to))
            {
                checkersDictionary[to]++;
            }
            else
            {
                checkersDictionary.Add(to, 1);
            }
        }

        private void RemoveCheckerFromDictionary(int from, Dictionary<int, int> checkersDictionary)
        {
            if (checkersDictionary[from] == 1)
            {
                checkersDictionary.Remove(from);
            }
            else
            {
                checkersDictionary[from]--;
            }
        }

        public IList<string> GetConectionStrings(string gameKey)
        {
            List<string> conections = new List<string>()
            {
                _boards[gameKey]._whiteConectionId,_boards[gameKey]._blackConectionId
            };
            return conections;
        }
    }
}