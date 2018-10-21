using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        #region User

        // Register. 
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

        // Login.
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

        // Logout.
        public void Logout(string userName)
        {
            _userManager.Logout(userName);
            _userManager.RemoveConectionId(userName);
            NotifyUserStateChanged();
        }

        // Return the contacts list to all clients.
        public Dictionary<string, UserState> GetContactList()
        {
            return _userManager._contactList;
        }

        // Return the updated contects list to all clients.
        public void NotifyUserStateChanged()
        {
            Clients.All.notifyUserStateChanged(_userManager._contactList);
        }

        #endregion


        #region Chat request
        // Send chat request to second user.
        public void SendChatRequest(string reciverName, string senderName)
        {
            string conectionId = _userManager.GetConectionId(reciverName);
            Clients.Client(conectionId).chatRequest(senderName);
        }

        // Get the response from the second user.
        public void HandleInvitationResult(bool response, string senderName, string reciverName)
        {
            if (response)
            {
                _userManager.AddNewPaier(senderName, reciverName);
            }
            string conectionId = _userManager.GetConectionId(senderName);
            Clients.Client(conectionId).getInvitationResult(response);
        }

        #endregion

        // Send message to second user.
        public void SendMessage(string message, string reciverName, string senderName)
        {
            string conectionId = _userManager.GetConectionId(reciverName);
            Clients.Client(conectionId).sendMessage(message, senderName);
        }


        // Change user state.
        public void ChangeUserStatus(string userName, UserState state)
        {
            _userManager.UpdateContactList(userName, state);
            NotifyUserStateChanged();
        }

        public void UserDisconnected(string userToChatWith)
        {
            _userManager.RemovePaier(userToChatWith);
            ChangeUserStatus(userToChatWith, UserState.online);

            string conectionId = _userManager.GetConectionId(userToChatWith);
            Clients.Client(conectionId).secondUserDisconnnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string currentUser = _userManager._userConections.FirstOrDefault((x) => x.Value == Context.ConnectionId).Key;
            ChangeUserStatus(currentUser, UserState.offline);
            string secondUser = _userManager.GetTheSecondUser(currentUser);
            if (secondUser != null)
            {
                ChangeUserStatus(secondUser, UserState.online);
                UserDisconnected(secondUser);
            }

            return base.OnDisconnected(stopCalled);
        }

        #region Game

        #endregion
    }
}