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


        //if to equals -2 it means he can take out from the board
        internal bool MoveChecker(int from, int to, string gameKey)
        {
            GameBoardState temp = _boards[gameKey];
            temp.IsBarred = false;
            bool isBlack = temp.CurrentPlayer == temp.BlackPlayer;

            if (to != -2)
            {
                if (!CorrectStep(temp, Math.Abs(from - to))) return false;
            }
            else
            {
                if (isBlack)//double
                {
                    if (temp.IsDouble)
                    {
                        if (temp.Dice.Die1 != -1 && temp.Dice.Die1 >= 24 - from)
                        {
                            temp._countMovments++;
                        }
                    }
                    else if (temp.Dice.Die1 != -1 && temp.Dice.Die1 >= 24 - from)//die =3 from = 24 - 21 
                    {
                        temp.Dice.Die1 = -1;
                    }
                    else if (temp.Dice.Die2 != -1 && temp.Dice.Die2 >= 24 - from)//die =3 from = 24 - 21 
                    {
                        temp.Dice.Die2 = -1;
                    }
                }
                else
                {
                    if (temp.IsDouble)
                    {
                        if (temp.Dice.Die1 != -1 && temp.Dice.Die1 > from)
                        {
                            temp._countMovments++;
                        }
                    }
                    else if (temp.Dice.Die1 != -1 && temp.Dice.Die1 > from)//from = 3  die = 4 true
                    {
                        temp.Dice.Die1 = -1;

                    }
                    else if (temp.Dice.Die2 != -1 && temp.Dice.Die2 > from)//from = 3  die = 4 true
                    {
                        temp.Dice.Die2 = -1;
                    }
                }
            }

            if (isBlack)// if is the black player.
            {
                if (temp.BarredBlackCheckers != 0)
                {
                    if (!PrisonerCanEscape(gameKey, temp))
                    {
                        temp.CurrentPlayer = temp.WhitePlayer;
                        temp.TurnChangaed = true;
                        return true;
                    }
                }

                if (to != -2 && to <= from || temp.BarredBlackCheckers != 0 && from != -1)
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

                if (to != -2)
                {
                    MoveCheckerInDictionary(to, temp.BlackCheckersLocation);
                }


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
                //if (temp.WhiteCanTakeOut)
                //{
                //    RemoveCheckerFromDictionary(from, temp.WhiteCheckersLocation);
                //    temp.MoveFrom = from;
                //    temp.MoveTo = to;
                //    _boards[gameKey] = temp;
                //    //Win();
                //    return true;
                //}
                if (temp.BarredWhiteCheckers != 0)
                {
                    if (!PrisonerCanEscape(gameKey, temp))
                    {
                        temp.CurrentPlayer = temp.BlackPlayer;
                        temp.TurnChangaed = true;
                        return true;
                    }
                }

                if (from <= to && temp.BarredWhiteCheckers != 0 && from != 24)
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

                if (to != -2)
                {
                    MoveCheckerInDictionary(to, temp.WhiteCheckersLocation);
                }

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

            CanTakeOut(temp, isBlack);

            temp.MoveFrom = from;
            temp.MoveTo = to;
            _boards[gameKey] = temp;
            return true;
        }

        internal void CanTakeOut(GameBoardState tempBoard, bool isBlack)
        {
            if (isBlack)
            {
                //tempBoard.BlackCanTakeOut = false;

                if (tempBoard.BarredBlackCheckers != 0)
                {
                    return;
                }

                for (int i = 0; i < 18; i++)
                {
                    if (tempBoard.BlackCheckersLocation.ContainsKey(i))
                    {
                        return;
                    }
                }
                tempBoard.BlackCanTakeOut = true;
            }
            else
            {
                //tempBoard.WhiteCanTakeOut = false;

                if (tempBoard.BarredWhiteCheckers != 0)
                {
                    return;
                }

                for (int i = 23; i > 5; i--)
                {
                    if (tempBoard.WhiteCheckersLocation.ContainsKey(i))
                    {
                        return;
                    }
                }
                tempBoard.WhiteCanTakeOut = true;
            }
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
                    if (gameBoard.Dice.Die1 == -1)
                    {
                        if (gameBoard.WhiteCheckersLocation.ContainsKey(gameBoard.Dice.Die2 - 1))
                        {
                            if (gameBoard.WhiteCheckersLocation[gameBoard.Dice.Die2 - 1] > 1)
                            {
                                gameBoard.MoveTo = -10;
                                return false;
                            }
                        }
                    }
                    else if (gameBoard.Dice.Die2 == -1)
                    {
                        if (gameBoard.WhiteCheckersLocation.ContainsKey(gameBoard.Dice.Die1 - 1))
                        {
                            if (gameBoard.WhiteCheckersLocation[gameBoard.Dice.Die1 - 1] > 1)
                            {
                                gameBoard.MoveTo = -10;
                                return false;
                            }
                        }
                    }
                    else if (gameBoard.WhiteCheckersLocation.ContainsKey(gameBoard.Dice.Die1 - 1) && gameBoard.WhiteCheckersLocation.ContainsKey(gameBoard.Dice.Die2 - 1))
                    {
                        if (gameBoard.WhiteCheckersLocation[gameBoard.Dice.Die1 - 1] > 1 && gameBoard.WhiteCheckersLocation[gameBoard.Dice.Die2 - 1] > 1)
                        {
                            gameBoard.MoveTo = -10;
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (gameBoard.BarredWhiteCheckers != 0)
                {
                    if (gameBoard.Dice.Die1 == -1)
                    {
                        if (gameBoard.BlackCheckersLocation.ContainsKey(24 - gameBoard.Dice.Die2))
                        {
                            if (gameBoard.BlackCheckersLocation[24 - gameBoard.Dice.Die2] > 1)
                            {
                                gameBoard.MoveTo = -10;
                                return false;
                            }
                        }
                    }
                    else if (gameBoard.Dice.Die2 == -1)
                    {
                        if (gameBoard.BlackCheckersLocation.ContainsKey(24 - gameBoard.Dice.Die1))
                        {
                            if (gameBoard.BlackCheckersLocation[24 - gameBoard.Dice.Die1] > 1)
                            {
                                gameBoard.MoveTo = -10;
                                return false;
                            }
                        }
                    }
                    else if (gameBoard.BlackCheckersLocation.ContainsKey(24 - gameBoard.Dice.Die1) && gameBoard.BlackCheckersLocation.ContainsKey(24 - gameBoard.Dice.Die2))
                    {
                        if (gameBoard.BlackCheckersLocation[24 - gameBoard.Dice.Die1] > 1 && gameBoard.BlackCheckersLocation[24 - gameBoard.Dice.Die2] > 1)
                        {
                            gameBoard.MoveTo = -10;
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
                tempBoard.WhiteCanTakeOut = false;
            }
            else
            {
                RemoveCheckerFromDictionary(to, tempBoard.BlackCheckersLocation);
                tempBoard.BarredBlackCheckers++;
                tempBoard.BlackCanTakeOut = false;
            }
            tempBoard.IsBarred = true;
        }

        private bool CorrectStep(GameBoardState tempBoard, int steps)
        {
            if (tempBoard.IsDouble)
            {
                if (tempBoard.Dice.Die1 == steps)
                {
                    tempBoard._countMovments++;
                    return true;
                }
                else
                {
                    return false;
                }//?
            }

            if (tempBoard.Dice.Die1 == steps)
            {
                tempBoard.Dice.Die1 = -1;
                return true;
            }
            if (tempBoard.Dice.Die2 == steps)
            {
                tempBoard.Dice.Die2 = -1;
                return true;
            }
            return false;
        }

        private void ReplaceTurn(GameBoardState tempBoard, string newTurn)
        {
            if (tempBoard.IsDouble)
            {
                if (tempBoard._countMovments == 4)
                {
                    tempBoard.CurrentPlayer = newTurn;
                    tempBoard._countMovments = 0;
                    tempBoard.TurnChangaed = true;
                }
                else
                {
                    tempBoard.TurnChangaed = false;
                }
                return;
            }
            if (tempBoard.Dice.Die1 == -1 && tempBoard.Dice.Die2 == -1)
            {
                tempBoard.CurrentPlayer = newTurn;
                tempBoard.TurnChangaed = true;
            }
            else
            {
                tempBoard.TurnChangaed = false;
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