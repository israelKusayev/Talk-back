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

        internal string GetConectionId(string reciverName)
        {
            string id = _userConections[reciverName];
            return id;
        }

        internal void AddConectionId(string connectionId, string userName)
        {
            try
            {
                _userConections.Add(userName, connectionId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal void RemoveConectionId(string userName)
        {
            _userConections.Remove(userName);
        }

        private ServerUserManager()
        {
            _contactList = new Dictionary<string, UserState>();
            _userConections = new Dictionary<string, string>();
            using (var context = new Db())
            {
                var usersInDb = context.UserTable.Where((e) => e.UserName != "");
                foreach (var user in usersInDb)
                {
                    _contactList.Add(user.UserName, UserState.offline);
                }
            }
        }



        internal void Logout(string userName)
        {
            UpdateContactList(userName, UserState.offline);
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

        internal bool Login(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UserName)
                           || string.IsNullOrWhiteSpace(user.Password)) return false;

            using (var context = new Db())
            {
                var thisUser = context.UserTable.FirstOrDefault(e => e.UserName == user.UserName);
                if (thisUser == null) return false;
            }
            UpdateContactList(user.UserName, UserState.online);
            return true;
        }
    }
}