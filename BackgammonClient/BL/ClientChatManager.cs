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
        private InitilaizeProxy _server = InitilaizeProxy.Instance;

        public ClientChatManager()
        {
            _server.Proxy.On("sendMessage", (string message, string senderName) =>
             {
                 _sendMessageEvent?.Invoke(message, senderName);
             });
            _server.HubConnection.Start().Wait();
        }

        public void RegisterSendMessageEvent(SendMessageEventHandler onSendEvent)
        {
            _sendMessageEvent += onSendEvent;
        }

        internal void InvokeSendMessage(string message)
        {
            Task task = Task.Run(async () =>
            {
                await _server.Proxy.Invoke("SendMessage", message, ClientUserManager.UserToChatWith, ClientUserManager.CurrentUser);
            });
            task.ConfigureAwait(false);
            task.Wait();
        }
    }
}
