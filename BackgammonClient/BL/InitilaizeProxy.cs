using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonClient.BL
{
    class InitilaizeProxy
    {
        public IHubProxy Proxy { get; set; }
        public HubConnection HubConnection { get; set; }

        private static InitilaizeProxy _instance;
        public static InitilaizeProxy Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InitilaizeProxy();
                }
                return _instance;
            }

            set
            {
                _instance = value;
            }
        }

        InitilaizeProxy()
        {
            HubConnection = new HubConnection("http://localhost:60762/");
            Proxy = HubConnection.CreateHubProxy("MainHub");
        }
    }
}
