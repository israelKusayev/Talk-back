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

        internal string currentTurn;

        private ServerGameManager()
        {
            _boards = new Dictionary<string, GameBoardState>();
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

        internal void InitializeBoard(string senderName, string reciverName)
        {
            string guid = senderName;
            GameBoardState gameBoard = new GameBoardState(senderName, reciverName);
            _boards.Add(guid, gameBoard);

        }

        internal IGameBoardState GetGameBoard(string gameKey)
        {
            return _boards[gameKey];
        }

        internal string GetGameKey(string senderName, string reciverName)
        {
            return _boards.ContainsKey(senderName) ? senderName : reciverName;
        }






        internal bool MoveChecker(int from, int to, string gameKey)
        {
            GameBoardState temp = _boards[gameKey];

            if (temp.CurrentPlayer == temp.BlackPlayer)
            {
                if (temp.WhiteCheckersLocation.ContainsKey(to))
                    return false;
                else
                {
                    MoveCheckerInDictionary(to, temp.BlackCheckersLocation);

                    RemoveCheckerFromDictionary(from, temp.BlackCheckersLocation);
                }
            }
            else
            {
                if (temp.BlackCheckersLocation.ContainsKey(to))
                    return false;

                MoveCheckerInDictionary(to, temp.BlackCheckersLocation);

                RemoveCheckerFromDictionary(from, temp.WhiteCheckersLocation);
            }
            temp.MoveFrom = from;
            temp.MoveTo = to;
            _boards[gameKey] = temp;
            return true;
        }

        private static void MoveCheckerInDictionary(int to, Dictionary<int, int> checkersDictionary)
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
                _boards[gameKey].WhiteConectionId,_boards[gameKey].BlackConectionId
            };
            return conections;
        }
    }
}