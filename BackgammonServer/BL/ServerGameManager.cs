using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using General.Models;

namespace BackgammonServer.BL
{
    public class ServerGameManager
    {
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

        }

        internal Dice RollDice()
        {
            Random rand = new Random();
            return new Dice() { Die1 = rand.Next(1, 7), Die2 = rand.Next(1,7) };
        }
    }
}