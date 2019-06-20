using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKHNNC
    {
    //[HubName("progressH")]
    public class ProgressHub : Hub
        {

            public string msg = "";
            public int count = 0;

            public static void SendMessage(string msg, int count)
            {
                var message =  msg;
                IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
                hub.Clients.All.sendMessage(string.Format(message), count);
                hub.Clients.All.messageRecived(message,count);
            }

            public void GetCountAndMessage()
            {
                Clients.Caller.sendMessage(string.Format(msg), count);
            }
        }
    }
