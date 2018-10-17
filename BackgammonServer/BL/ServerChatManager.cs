using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgammonServer.BL
{
    public class ServerChatManager
    {
        internal Dictionary<string, string> _userConections { get; private set; }

        internal void SendMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}