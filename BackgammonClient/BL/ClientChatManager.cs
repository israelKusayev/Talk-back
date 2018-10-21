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
    public delegate void ChatRequestEventHandler();
    public delegate void UserDisconnectedEventHandler();

    class ClientChatManager
    {
        #region Events
        private event SendMessageEventHandler _sendMessageEvent;
        private event InvitationResponseEventHandler _invitationResponseEvent;
        private event ChatRequestEventHandler _ChatRequestEvent;
        private event UserDisconnectedEventHandler _userDisconnectedEvent;
        #endregion 

        private InitilaizeProxy _server = InitilaizeProxy.Instance;

        public ClientChatManager()
        {
            #region Proxy.On


            //for reciver
            _server.Proxy.On("chatRequest", (string senderName) =>
            {
                //Application.Current.Dispatcher.Invoke(() =>
                //{

                //});
                MessageBoxResult result = MessageBox.Show($"{senderName} invite you to chat", "Chat request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                bool boolResult = result == MessageBoxResult.Yes ? true : false;
                if (boolResult)
                {
                    ClientUserManager.UserToChatWith = senderName;
                    _ChatRequestEvent?.Invoke();//open reciver chat.
                }
                else
                {
                    // todo?
                }
                Task task = Task.Run(async () =>
                {
                    await _server.Proxy.Invoke("HandleInvitationResult", result, senderName, ClientUserManager.CurrentUser);
                });
                task.ConfigureAwait(false);
                //task.Wait();//worker thread.
            });


            //for sender
            _server.Proxy.On("getInvitationResult", (bool userResponse) =>
            {
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
        internal void SendChatRequest()
        {
            Task task = Task.Run(async () =>
            {
                await _server.Proxy.Invoke("SendChatRequest", ClientUserManager.UserToChatWith, ClientUserManager.CurrentUser);
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
        public void RegisterChatRequestEvent(ChatRequestEventHandler onChatRequestEvent)
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
