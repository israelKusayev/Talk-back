using BackgammonClient.Models;
using General.Emuns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.Converters
{
    public class ConvertUserForUserView
    {
        public static ObservableCollection<UserForView> ConvertUser(Dictionary<string, UserState> users)
        {
            ObservableCollection<UserForView> NewUsers = new ObservableCollection<UserForView>();
            foreach (var item in users)
            {
                UserForView temp = new UserForView() { UserName = item.Key, State = item.Value };
                NewUsers.Add(temp);
            }
            return NewUsers;
        }
    }
}
