using BackgammonServer.DAL;
using BackgammonServer.Models;
using General.Emuns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BackgammonServer.BL
{
    public class ServerUserManager
    {
        internal Dictionary<string, UserState> _contactList { get; private set; }
        internal Dictionary<string, string> _userConections { get; private set; }
        internal Dictionary<string, string> _interactingUsersPairs { get; set; }
        private static ServerUserManager _instance;
        public static ServerUserManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServerUserManager();
                }
                return _instance;
            }
        }

        private ServerUserManager()
        {
            _contactList = new Dictionary<string, UserState>();
            _userConections = new Dictionary<string, string>();
            _interactingUsersPairs = new Dictionary<string, string>();
            using (var context = new Db())
            {
                var usersInDb = context.UserTable.Where((e) => e.UserName != "");
                foreach (var user in usersInDb)
                {
                    _contactList.Add(user.UserName, UserState.offline);
                }
            }
        }

        internal string GetConectionId(string reciverName)
        {
            return _userConections[reciverName];
        }

        internal void AddConectionId(string connectionId, string userName)
        {
            if (!_userConections.ContainsKey(userName))
            {
                _userConections.Add(userName, connectionId);
            }
            else
            {
                //todo?
            }
        }

        internal void RemoveConectionId(string userName)
        {
            _userConections.Remove(userName);
        }

        public void UpdateContactList(string username, UserState newState)
        {
            if (_contactList.ContainsKey(username))
            {
                _contactList[username] = newState;
            }
            else
            {
                _contactList.Add(username, newState);
            }
        }


        public bool RegisterToDb(User user)
        {

            if (user == null || string.IsNullOrWhiteSpace(user.UserName)
                || string.IsNullOrWhiteSpace(user.Password)) return false;

            using (var context = new Db())
            {
                var thisUser = context.UserTable.FirstOrDefault(e => e.UserName == user.UserName);
                if (thisUser != null) return false;

                context.UserTable.Add(user);
                context.SaveChanges();
            }
            UpdateContactList(user.UserName, UserState.online);

            return true;
        }


        internal string Login(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
            { return "You must fill the username and password fields."; }

            using (var context = new Db())
            {
                User thisUser = context.UserTable.FirstOrDefault(e => e.UserName == user.UserName);
                if (thisUser == null || thisUser.Password != user.Password)
                {
                    return "username or password is incorrect.";
                }
            }
            if (_contactList[user.UserName] == UserState.online)
            {
                return "You are already logged in.";
            }
            UpdateContactList(user.UserName, UserState.online);
            return null;
        }

        internal void Logout(string userName)
        {
            UpdateContactList(userName, UserState.offline);
        }

        internal void AddNewPaier(string senderName, string reciverName)
        {
            _interactingUsersPairs.Add(senderName, reciverName);
        }


        internal void RemovePaier(string userToChatWith)
        {
            bool result = _interactingUsersPairs.Remove(userToChatWith);
            if (!result)
            {
                string key = _interactingUsersPairs.FirstOrDefault((x) => x.Value == userToChatWith).Key;
                _interactingUsersPairs.Remove(key);
            }
        }

        internal string GetTheSecondUser(string currentUser)
        {
            if (_interactingUsersPairs.ContainsKey(currentUser))
            {
                return _interactingUsersPairs[currentUser];
            }
            else if (_interactingUsersPairs.ContainsValue(currentUser))
            {
                return _interactingUsersPairs.FirstOrDefault((x) => x.Value == currentUser).Key;
            }
            return null;
        }
    }
}