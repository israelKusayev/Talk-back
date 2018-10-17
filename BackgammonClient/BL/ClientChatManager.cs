using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.BL
{
    public delegate void SendMessageEventHandler(string message, string senderName);
    class ClientChatManager
    {
        private event SendMessageEventHandler _sendMessageEvent;


        private InitilaizeProxy initilaize = InitilaizeProxy.Instance;

        public ClientChatManager()
        {

            initilaize.Proxy.On("brodcastMessage", (string message, string senderName) =>
             {
                 _sendMessageEvent?.Invoke(message, senderName);
             });
            initilaize.HubConnection.Start().Wait();
        }

        public void RegisterSendMessageEvent(SendMessageEventHandler onSendEvent)
        {
            _sendMessageEvent += onSendEvent;
        }

        internal void InvokeSendMessage(string message)
        {
            Task task = Task.Run(async () =>
            {
                await initilaize.Proxy.Invoke("SendMessage", message, ClientUserManager.UserToChatWith, ClientUserManager.CurrentUser);
            });
            task.ConfigureAwait(false);
            task.Wait();
        }
    }
}
