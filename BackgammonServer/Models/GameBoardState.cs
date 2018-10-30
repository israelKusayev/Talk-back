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

        //black is asc// white is desc;
        public Dictionary<int, int> BlackCheckersLocation { get; set; }
        public Dictionary<int, int> WhiteCheckersLocation { get; set; }
        public Dice Dice { get; set; }
        public bool IsDouble { get; set; }
        public string CurrentPlayer { get; set; }
        public int BarredBlackCheckers { get; set; }
        public int BarredWhiteCheckers { get; set; }
        public string BlackPlayer { get; set; }
        public string WhitePlayer { get; set; }
        public int MoveFrom { get; set; }
        public int MoveTo { get; set; }
        public bool TurnChangaed { get; set; }

        internal readonly string _blackConectionId;
        internal readonly string _whiteConectionId;
        internal int _countMovments;
        private ServerUserManager _userManager = ServerUserManager.Instance;
        public GameBoardState(string whitePlayer, string blackPlayer)
        {
            CurrentPlayer = WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;

            _blackConectionId = _userManager.GetConectionId(blackPlayer);
            _whiteConectionId = _userManager.GetConectionId(whitePlayer);

            InitializeBoardGame();
        }

        public GameBoardState()
        {
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