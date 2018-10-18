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
        //ServerChatManager _chatManager;

        public MainHub()
        {
            //_chatManager = new ServerChatManager();
        }

        #region User
        public bool Register(User newUser)
        {
            if (_userManager.RegisterToDb(newUser))
            {
                _userManager.AddConectionId(Context.ConnectionId, newUser.UserName);
                NotifyUserStateChanged();
                return true;
            }
            return false;
        }
        public bool Login(User user)
        {
            if (_userManager.Login(user))
            {
                _userManager.AddConectionId(Context.ConnectionId, user.UserName);
                NotifyUserStateChanged();
                return true;
            }
            return false;
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
            Clients.Client(conectionId).sendMessage(message, senderName);
        }

        public void ChangeUserStatus(string userName, UserState state)
        {
            _userManager.UpdateContactList(userName, state);
            NotifyUserStateChanged();
        }
    }
}