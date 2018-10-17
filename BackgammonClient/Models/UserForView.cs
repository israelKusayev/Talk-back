using General.Emuns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.Models
{
   public class UserForView
    {
        public string UserName { get; set; }
        public UserState State { get; set; }
        public string ConectionID { get; set; }
    }
}
