using BackgammonServer.BL;
using General.Interfaces;
using General.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgammonServer.Models
{
    public class GameBoardState : IGameBoardState
    {
        public Dictionary<int, int> BlackCheckersLocation { get; set; }
        public Dictionary<int, int> WhiteCheckersLocation { get; set; }
        public Dice Dice { get; set; }
        public bool IsDouble { get; set; }
        public string CurrentPlayer { get; set; }
        public int BarredBlackCheckers { get; set; }
        public int BarredWhiteCheckers { get; set; }

        internal readonly string BlackPlayer;
        internal readonly string WhitePlayer;
        internal readonly string BlackConectionId;
        internal readonly string WhiteConectionId;

        private ServerUserManager _userManager = ServerUserManager.Instance;
        public GameBoardState(string whitePlayer, string blackPlayer)
        {
            CurrentPlayer = WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;

            BlackConectionId = _userManager.GetConectionId(blackPlayer);
            WhiteConectionId = _userManager.GetConectionId(whitePlayer);

            InitializeBoardGame();
        }

        private void InitializeBoardGame()
        {
            BlackCheckersLocation = new Dictionary<int, int>()
            {
                {0,2},
                {11,5},
                {16,3},
                {18,5}
            };
            WhiteCheckersLocation = new Dictionary<int, int>()
            {
                {5,5},
                {7,3},
                {12,5},
                {23,2}
            };
        }
    }
}