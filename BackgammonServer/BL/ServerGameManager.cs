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
        private Dictionary<string, IGameBoardState> _boards;
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
            _boards = new Dictionary<string, IGameBoardState>();
        }

        internal Dice RollDice()
        {
            Random rand = new Random();
            return new Dice() { Die1 = rand.Next(1, 7), Die2 = rand.Next(1, 7) };
        }

        internal string InitializeBoard(string senderName, string reciverName)
        {
            string guid = Guid.NewGuid().ToString();
            GameBoardState gameBoard = new GameBoardState(senderName, reciverName);
            _boards.Add(guid, gameBoard);
            return guid;
        }

        internal IGameBoardState GetGameBoard(string gameKey)
        {
            return _boards[gameKey];
        }
    }
}