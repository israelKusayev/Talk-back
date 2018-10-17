using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.Utils
{
    class ChatMessageFormatter
    {
        public static string Format(string message, string senderName)
        {
            return $"[{DateTime.Now}] {senderName}: {message} \n";
        }
    }
}
