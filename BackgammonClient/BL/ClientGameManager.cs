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
    delegate void BoardUpdatedEventHandler();

    class ClientGameManager
    {
        private string _gameKey;

        internal GameBoardState _gameBoard;
        private InitilaizeProxy _server = InitilaizeProxy.Instance;


        private event GetDiceEventHandler _getDiceEvent;
        private event BoardUpdatedEventHandler _boardUpdatedEvent;
        public ClientGameManager()
        {
            _server.Proxy.On("getDiceResult", (Dice dice) =>
             {
                 _getDiceEvent?.Invoke(dice);
             });

            _server.Proxy.On("getUpdatedBoard", (GameBoardState updatedGameBoard) =>
            {
                _gameBoard = updatedGameBoard;
                _boardUpdatedEvent?.Invoke();

            });
            _server.HubConnection.Start().Wait();
            Task<string> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<string>("GetGameKey", ClientUserManager.CurrentUser, ClientUserManager.UserToChatWith);
            });
            _gameKey = task.Result;

        }

        internal Dice RollDice()
        {
            Task<Dice> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<Dice>("RollDice", _gameKey);
            });
            return task.Result;
        }

        public void RegisterGetDiceEvent(GetDiceEventHandler getDiceEvent)
        {
            _getDiceEvent += getDiceEvent;
        }
        public void RegisterBoardUpdatedEvent(BoardUpdatedEventHandler boardUpdatedEvent)
        {
            _boardUpdatedEvent += boardUpdatedEvent;
        }

        internal GameBoardState GetBoardState()
        {
            Task<GameBoardState> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<GameBoardState>("GetGameBoard", _gameKey);
            });
            _gameBoard = task.Result;
            return _gameBoard;
        }

        internal bool MoveChecker(int selectedChecker, int selectedLocation)
        {
            Task<bool> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<bool>("MoveChecker", selectedChecker, selectedLocation, _gameKey);
            });

            return task.Result;
        }

        internal bool ValidateCheckerColor(int selectedChecker)
        {
            if (_gameBoard.CurrentPlayer == _gameBoard.WhitePlayer)
            {
                if (_gameBoard.WhiteCheckersLocation.ContainsKey(selectedChecker))
                {
                    return true;
                }
            }
            else
            {
                if (_gameBoard.BlackCheckersLocation.ContainsKey(selectedChecker))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

