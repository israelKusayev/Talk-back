using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BackgammonServer.BL;
using Microsoft.AspNet.SignalR;

namespace BackgammonServer.Hubs
{
    public class ChatHub : Hub
    {
        //public void SendMessage(string message, string reciverName,string senderName)
        //{
        //    string conectionId = _userManager.GetConectionId(reciverName);
        //    Clients.Client(conectionId).brodcastMessage(message, senderName);
        //}

       
    }
}