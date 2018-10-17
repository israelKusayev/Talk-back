using BackgammonClient.Models;
using General.Emuns;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.BL
{
    public delegate void NotifyStateEventHandler(Dictionary<string, UserState> dictionary);
    class ClientUserManager
    {
        private event NotifyStateEventHandler NotifyEvent;

        private InitilaizeProxy initilaize = InitilaizeProxy.Instance;
        public static string CurrentUser { get; set; }
        public static string UserToChatWith { get; set; }

        public ClientUserManager()
        {
            initilaize.Proxy.On("notifyUserStateChanged", (Dictionary<string, UserState> updatedContactList) =>
            {
                updatedContactList.Remove(CurrentUser);
                NotifyEvent?.Invoke(updatedContactList);
            });

            initilaize.HubConnection.Start().Wait();
        }


        public void RegisterNotifyEvent(NotifyStateEventHandler onNotifyEvent)
        {
            NotifyEvent += onNotifyEvent;
        }



        internal Dictionary<string, UserState> GetContactList()
        {
            Task<Dictionary<string, UserState>> Contacts = Task.Run(async () =>
           {
               return await initilaize.Proxy.Invoke<Dictionary<string, UserState>>("GetContactList");
           });
            Contacts.ConfigureAwait(false);
            Contacts.Wait();

            Contacts.Result.Remove(CurrentUser);
            return Contacts.Result;
        }

        internal void InvokeLogout()
        {
            Task task = Task.Run(async () =>
            {
                await initilaize.Proxy.Invoke("Logout", CurrentUser);
            });
            task.ConfigureAwait(false);
            task.Wait();
        }

        internal void ChangeUserStatus(UserState state)
        {
            Task task = Task.Run(async () =>
            {
                await initilaize.Proxy.Invoke("ChangeUserStatus", CurrentUser, state);
            });
            task.ConfigureAwait(false);
            task.Wait();
        }

        internal void InvokeRegister(User user)
        {
            try
            {
                CurrentUser = user.UserName;
                Task task = Task.Run(async () =>
                {
                    await initilaize.Proxy.Invoke("Register", user);
                });
                task.ConfigureAwait(false);
                task.Wait();
            }
            catch (Exception)
            {
                throw;
            }

        }

        internal void InvokeLogin(User user)
        {
            try
            {
                CurrentUser = user.UserName;
                Task task = Task.Run(async () =>
                {
                    await initilaize.Proxy.Invoke("Login", user);
                });
                task.ConfigureAwait(false);
                task.Wait();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
