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

            if (temp.CurrentPlayer == temp.BlackPlayer)// if is the black player.
            {

                if (to <= from || temp.WhiteCheckersLocation.ContainsKey(to)
                    || !CorrectStep(temp, Math.Abs(from - to)))// if he want to move checker on rival checker.
                {
                    return false;
                }
                else
                {
                    MoveCheckerInDictionary(to, temp.BlackCheckersLocation);

                    RemoveCheckerFromDictionary(from, temp.BlackCheckersLocation);

                    ReplaceTurn(temp, temp.WhitePlayer);

                }
            }
            else // if is the white player.
            {
                if (from <= to || temp.BlackCheckersLocation.ContainsKey(to)
                    || !CorrectStep(temp, Math.Abs(from - to)))
                {
                    return false;
                }

                MoveCheckerInDictionary(to, temp.WhiteCheckersLocation);

                RemoveCheckerFromDictionary(from, temp.WhiteCheckersLocation);

                ReplaceTurn(temp, temp.BlackPlayer);
            }
            temp.MoveFrom = from;
            temp.MoveTo = to;
            _boards[gameKey] = temp;
            return true;
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