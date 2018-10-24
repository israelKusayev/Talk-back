using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General.Models;
using Microsoft.AspNet.SignalR.Client;

namespace BackgammonClient.BL
{
    delegate void GetDiceEventHandler(Dice dice);
    class ClientGameManager
    {
        public static string GameKey { get; set; }
        
        internal static bool isMyTurn;//?
        private InitilaizeProxy _server = InitilaizeProxy.Instance;


        private event GetDiceEventHandler GetDiceEvent;
        public ClientGameManager()
        {
            _server.Proxy.On("getDiceResult", (Dice dice) =>
             {
                 GetDiceEvent?.Invoke(dice);
             });
            _server.HubConnection.Start().Wait();
        }

        internal Dice RollDice()
        {
            Task<Dice> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<Dice>("RollDice", ClientUserManager.UserToChatWith);
            });
            return task.Result;
        }

        public void RegisterGetDiceEvent(GetDiceEventHandler getDiceEvent)
        {
            GetDiceEvent += getDiceEvent;
        }
    }
}
