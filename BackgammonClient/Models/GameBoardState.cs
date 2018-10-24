using General.Interfaces;
using General.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.Models
{
    class GameBoardState : IGameBoardState
    {
        public Dictionary<int, int> BlackCheckersLocation { get ; set ; }
        public Dictionary<int, int> WhiteCheckersLocation { get ; set ; }
        public Dice Dice { get ; set ; }
        public bool IsDouble { get ; set ; }
        public string CurrentPlayer { get ; set ; }
        public int BarredBlackCheckers { get ; set ; }
        public int BarredWhiteCheckers { get ; set ; }
    }
}
