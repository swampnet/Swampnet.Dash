using Microsoft.AspNet.SignalR;
using Swampnet.Dash.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Service.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ITestService _test;

        public ChatHub(ITestService test)
        {
            _test = test;
        }


        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage($"{name} ({_test.Id})", message);
        }
    }
}
