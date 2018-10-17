using General.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgammonServer.Models
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}