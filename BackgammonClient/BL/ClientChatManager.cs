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
    public delegate void InvitationResponseEventHandler(bool response);
    public delegate void InteractionRequestEventHandler(bool isChat);
    public delegate void UserDisconnectedEventHandler();

    class ClientChatManager
    {
        #region Events
        private event SendMessageEventHandler _sendMessageEvent;
        private event InvitationResponseEventHandler _invitationResponseEvent;
        private event InteractionRequestEventHandler _ChatRequestEvent;
        private event UserDisconnectedEventHandler _userDisconnectedEvent;
        #endregion 

        private InitilaizeProxy _server = InitilaizeProxy.Instance;

        public ClientChatManager()
        {
            #region Proxy.On


            //for reciver
            _server.Proxy.On("InteractionRequest", (string senderName, bool isChat) =>
            {
                string gameOrChat = isChat ? "chat" : "game";
                MessageBoxResult result = MessageBox.Show($"{senderName} invite you to {gameOrChat}", $"{gameOrChat} request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                bool boolResult = result == MessageBoxResult.Yes ? true : false;
                if (boolResult)
                {
                    if (!isChat)
                    {
                        Task<string> t = Task.Run(async () =>
                        {
                            return await _server.Proxy.Invoke<string>("InitializeBoardGame", senderName, ClientUserManager.CurrentUser);
                        });
                        t.ConfigureAwait(false);
                        ClientGameManager.GameKey = t.Result;
                    }
                    ClientUserManager.UserToChatWith = senderName;
                    _ChatRequestEvent?.Invoke(isChat);//open reciver chat/game.
                }
                else
                {
                    // todo?
                }
                Task task = Task.Run(async () =>
                {
                    await _server.Proxy.Invoke("HandleInvitationResult", result, senderName, ClientUserManager.CurrentUser, ClientGameManager.GameKey);
                });
                task.ConfigureAwait(false);
                //task.Wait();//worker thread.
            });


            //for sender
            _server.Proxy.On("getInvitationResult", (bool userResponse, string gameKey) =>
            {
                ClientGameManager.GameKey = gameKey;
                _invitationResponseEvent?.Invoke(userResponse);
            });

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
            #endregion

            _server.HubConnection.Start().Wait();
        }

        //Send chat request to second user.
        internal void SendRequest(bool isChat)
        {
            Task task = Task.Run(async () =>
            {
                await _server.Proxy.Invoke("SendRequest", ClientUserManager.UserToChatWith, ClientUserManager.CurrentUser, isChat);
            });
            task.ConfigureAwait(false);
            task.Wait();
        }

        //Send message to second user.
        internal void InvokeSendMessage(string message)
        {
            Task task = Task.Run(async () =>
            {
                await _server.Proxy.Invoke("SendMessage", message, ClientUserManager.UserToChatWith, ClientUserManager.CurrentUser);
            });
            //task.ConfigureAwait(false);
            //task.Wait();
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
        public void RegisterInvitationResponseEvent(InvitationResponseEventHandler HandleResponse)
        {
            _invitationResponseEvent += HandleResponse;
        }
        public void RegisterChatRequestEvent(InteractionRequestEventHandler onChatRequestEvent)
        {
            _ChatRequestEvent += onChatRequestEvent;
        }
        public void RegisterUserDisconnectedEvent(UserDisconnectedEventHandler onUserDisconnectedEvent)
        {
            _userDisconnectedEvent += onUserDisconnectedEvent;
        }
        #endregion

    }
}
