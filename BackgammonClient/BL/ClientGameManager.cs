using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackgammonClient.Models;
using General.Interfaces;
using General.Models;
using Microsoft.AspNet.SignalR.Client;

namespace BackgammonClient.BL
{
    delegate void GetDiceEventHandler(Dice dice);
    class ClientGameManager
    {
        public string GameKey { get; set; }

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
            Task<string> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<string>("GetGameKey", ClientUserManager.CurrentUser, ClientUserManager.UserToChatWith);

            });
            GameKey = task.Result;
            /*.ContinueWith(continuedTask => GameKey = continuedTask.Result);*/

        }

        async Task<string> GetKey()
        {
            return await _server.Proxy.Invoke<string>("GetGameKey", ClientUserManager.CurrentUser, ClientUserManager.UserToChatWith);
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

        internal GameBoardState GetBoardState()
        {
            Task<GameBoardState> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<GameBoardState>("GetGameBoard", GameKey);
            });

            task.Wait();//?
            return task.Result;
        }
    }
}

