using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackgammonClient.BL
{
    public delegate void InvitationResponseEventHandler(bool response);
    public delegate void InteractionRequestEventHandler(bool isChat);
    class ClientInteractionManager
    {
        private InitilaizeProxy _server = InitilaizeProxy.Instance;

        private event InvitationResponseEventHandler _invitationResponseEvent;
        private event InteractionRequestEventHandler _ChatRequestEvent;
        public ClientInteractionManager()
        {
            _server.Proxy.On("InteractionRequest", (string senderName, bool isChat) =>
            {
                string gameOrChat = isChat ? "chat" : "game";
                MessageBoxResult result = default(MessageBoxResult);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    result = MessageBox.Show(Application.Current.MainWindow, $"{senderName} invite you to {gameOrChat}", $"{gameOrChat} request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }));
                bool boolResult = result == MessageBoxResult.Yes ? true : false;
                if (boolResult)
                {
                    if (!isChat)
                    {
                        Task t = Task.Run(async () =>
                        {
                            await _server.Proxy.Invoke("InitializeBoardGame", senderName, ClientUserManager.CurrentUser);
                        });
                        t.ConfigureAwait(false);
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
                    await _server.Proxy.Invoke("HandleInvitationResult", boolResult, senderName, ClientUserManager.CurrentUser);
                });
            });

            //for sender
            _server.Proxy.On("getInvitationResult", (bool userResponse) =>
            {
                _invitationResponseEvent?.Invoke(userResponse);
            });

            _server.HubConnection.Start().Wait();
        }

        //Send chat request to second user.
        internal void SendRequest(bool isChat)
        {
            Task task = Task.Run(async () =>
            {
                await _server.Proxy.Invoke("SendRequest", ClientUserManager.UserToChatWith, ClientUserManager.CurrentUser, isChat);
            });
        }

        public void RegisterInvitationResponseEvent(InvitationResponseEventHandler HandleResponse)
        {
            _invitationResponseEvent += HandleResponse;
        }
        public void RegisterChatRequestEvent(InteractionRequestEventHandler onChatRequestEvent)
        {
            _ChatRequestEvent += onChatRequestEvent;
        }
    }
}
