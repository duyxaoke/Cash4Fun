using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Configuration;
using Microsoft.AspNet.SignalR.Hubs;

namespace CashMe.Admin.Hubs
{
    public class WalletHub : Hub
    {
        private static string conString =
        ConfigurationManager.ConnectionStrings["DbConnect"].ToString();
        public void Hello()
        {
            Clients.All.hello();
        }

        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<WalletHub>();
            context.Clients.All.updateMessages();
        }
    }
}