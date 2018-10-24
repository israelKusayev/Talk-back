using General.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Interfaces
{
    public interface IGameBoardState
    {
        Dictionary<int, int> BlackCheckersLocation { get; set; }

        Dictionary<int, int> WhiteCheckersLocation { get; set; }

        Dice Dice { get; set; }

        bool IsDouble { get; set; }

        string CurrentPlayer { get; set; }

        int BarredBlackCheckers { get; set; }

        int BarredWhiteCheckers { get; set; }
    }
}
