using BackgammonClient.Models;
using General.Emuns;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackgammonClient.BL
{
    public delegate void NotifyStateEventHandler(Dictionary<string, UserState> dictionary);
    public class ClientUserManager
    {
        private event NotifyStateEventHandler NotifyEvent;
        private InitilaizeProxy _server = InitilaizeProxy.Instance;
        public static string CurrentUser { get; set; }
        public static string UserToChatWith { get; set; }

        public ClientUserManager()
        {

            _server.Proxy.On("notifyUserStateChanged", (Dictionary<string, UserState> updatedContactList) =>
            {
                if (CurrentUser != null)
                {
                    updatedContactList.Remove(CurrentUser);
                }
                NotifyEvent?.Invoke(updatedContactList);
            });
            _server.HubConnection.Start().Wait();

        }


        public void RegisterNotifyEvent(NotifyStateEventHandler onNotifyEvent)
        {
            NotifyEvent += onNotifyEvent;
        }

        internal Dictionary<string, UserState> GetContactList()
        {
            Task<Dictionary<string, UserState>> Contacts = Task.Run(async () =>
           {
               return await _server.Proxy.Invoke<Dictionary<string, UserState>>("GetContactList");
           });

            Contacts.Result.Remove(CurrentUser);
            return Contacts.Result;
        }

        internal void ChangeUserStatus(UserState state)
        {
            Task task = Task.Run(async () =>
            {
                await _server.Proxy.Invoke("ChangeUserStatus", CurrentUser, state);
            });
            task.ConfigureAwait(false);
        }

        internal bool InvokeRegister(User user)
        {
            try
            {
                CurrentUser = user.UserName;
                Task<bool> task = Task.Run(async () =>
                {
                    return await _server.Proxy.Invoke<bool>("Register", user);

                });
                if (!task.Result)
                {
                    MessageBox.Show("this username already exists.");
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("server error please try again.");
                return false;
            }

        }

        internal bool InvokeLogin(User user)
        {
            CurrentUser = user.UserName;
            Task<string> task = Task.Run(async () =>
                {
                    return await _server.Proxy.Invoke<string>("Login", user);
                });
            if (task.Result != null)
            {
                MessageBox.Show(task.Result);
                return false;
            }
            return true;
        }

        internal bool InvokeLogout()
        {
            try
            {
                Task task = Task.Run(async () =>
                {
                    await _server.Proxy.Invoke("Logout", CurrentUser);
                });
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("server error please try again.");
                return false;
            }
        }
    }
}
