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
        private InitilaizeProxy _server = InitilaizeProxy.Instance;
        private readonly string _gameKey;
        internal GameBoardState _gameBoard;


        private event GetDiceEventHandler _getDiceEvent;
        private event BoardUpdatedEventHandler _boardUpdatedEvent;


        private static ClientGameManager _Instance;

        public static ClientGameManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ClientGameManager();
                }
                return _Instance;
            }
            set { _Instance = value; }
        }
        //ctor
        private ClientGameManager()
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

            _gameKey = GetGameKey();
        }

        private string GetGameKey()
        {
            Task<string> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<string>("GetGameKey", ClientUserManager.CurrentUser, ClientUserManager.UserToChatWith);
            });
            return task.Result;
        }

        internal Dice RollDice()
        {
            Task<Dice> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<Dice>("RollDice", _gameKey);
            });
            _gameBoard.Dice = task.Result;
            return task.Result;
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

        internal bool MoveChecker(int selectedChecker, int selectedLocation)
        {
            Task<bool> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<bool>("MoveChecker", selectedChecker, selectedLocation, _gameKey);
            });
            return task.Result;
        }

        internal void RegisterGetDiceEvent(GetDiceEventHandler getDiceEvent)
        {
            _getDiceEvent += getDiceEvent;
        }
        internal void RegisterBoardUpdatedEvent(BoardUpdatedEventHandler boardUpdatedEvent)
        {
            _boardUpdatedEvent += boardUpdatedEvent;
        }

        internal bool PrisonerCanEscape()
        {
            Task<bool> task = Task.Run(async () =>
            {
                return await _server.Proxy.Invoke<bool>("PrisonerCanEscape", _gameKey);
            });
            return task.Result;
        }
    }
}

