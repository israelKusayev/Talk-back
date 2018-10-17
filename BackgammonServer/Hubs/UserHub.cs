using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BackgammonServer.BL;
using BackgammonServer.Models;
using General.Emuns;
using Microsoft.AspNet.SignalR;

namespace BackgammonServer.Hubs
{
    public class MainHub : Hub
    {
        ServerUserManager _userManager = ServerUserManager.Instance;
        ServerChatManager _chatManager;

        public MainHub()
        {
            _chatManager = new ServerChatManager();
        }

        #region User
        public void Register(User newUser)
        {
            _userManager.RegisterToDb(newUser);
            _userManager.AddConectionId(Context.ConnectionId, newUser.UserName);
            NotifyUserStateChanged();
        }
        public void Login(User user)
        {
            _userManager.Login(user);
            _userManager.AddConectionId(Context.ConnectionId, user.UserName);
            NotifyUserStateChanged();
        }

        public void Logout(string userName)
        {
            _userManager.Logout(userName);
            _userManager.RemoveConectionId(userName);
            NotifyUserStateChanged();
        }

        public void NotifyUserStateChanged()
        {
            Clients.All.notifyUserStateChanged(_userManager._contactList);
        }

        public Dictionary<string, UserState> GetContactList()
        {
            return _userManager._contactList;
        }

        #endregion


        public void SendMessage(string message, string reciverName, string senderName)
        {
            string conectionId = _userManager.GetConectionId(reciverName);
            Clients.Client(conectionId).brodcastMessage(message, senderName);
        }

        public void ChangeUserStatus(string userName, UserState state)
        {
            _userManager.UpdateContactList(userName, state);
            NotifyUserStateChanged();
        }
    }
}