using BackgammonClient.Views;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BackgammonClient.BL
{
    public delegate void SendMessageEventHandler(string message, string senderName);

    public delegate void UserDisconnectedEventHandler();
    class ClientChatManager
    {
        #region Events
        private event SendMessageEventHandler _sendMessageEvent;
        
        private event UserDisconnectedEventHandler _userDisconnectedEvent;
        #endregion 

        private InitilaizeProxy _server = InitilaizeProxy.Instance;

        public ClientChatManager()
        {
            // recive message from user.
            _server.Proxy.On("sendMessage", (string message, string senderName) =>
            {
                _sendMessageEvent?.Invoke(message, senderName);
            });
            _server.Proxy.On("secondUserDisconnnected", () =>
            {
                Task task = Task.Run(() =>
                {
                    _userDisconnectedEvent?.Invoke();
                });
            });
            _server.HubConnection.Start().Wait();
        }
    
        //Send message to second user.
        internal void InvokeSendMessage(string message)
        {
            Task task = Task.Run(async () =>
            {
                await _server.Proxy.Invoke("SendMessage", message, ClientUserManager.UserToChatWith, ClientUserManager.CurrentUser);
            });
        }

        internal void UserDisconnected()
        {
            Task task = Task.Run(async () =>
            {
                await _server.Proxy.Invoke("UserDisconnected", ClientUserManager.UserToChatWith);
            });

        }

        #region Register events
        public void RegisterSendMessageEvent(SendMessageEventHandler onSendEvent)
        {
            _sendMessageEvent += onSendEvent;
        }
      
        public void RegisterUserDisconnectedEvent(UserDisconnectedEventHandler onUserDisconnectedEvent)
        {
            _userDisconnectedEvent += onUserDisconnectedEvent;
        }
        #endregion

    }
}
