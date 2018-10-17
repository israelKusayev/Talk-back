using BackgammonServer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BackgammonServer.DAL
{
    public class Db:DbContext
    {
        public DbSet<User> UserTable { get; set; }
    }
}