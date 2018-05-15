using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace CashMe.Admin.Hubs
{
    public class TrackingHub : Hub
    {
        public void SendMessage(bool status)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<TrackingHub>();
            context.Clients.All.message(status);
        }
    }
}