using Microsoft.AspNet.SignalR;
using Serilog;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using Swampnet.Dash.Service.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Service.Services
{
    class Broadcast : IBroadcast
    {
        public void DashboardItems(string group, IEnumerable<DashItem> dashItems)
        {
            Log.Debug("Broadcast to '{group}' - {dashItems}",
                group,
                string.Join(", ", dashItems.Select(di => di.Id)));

            GlobalHost.ConnectionManager
                .GetHubContext<DashboardHub>()
                .Clients
                .Group(group)
                .updateDashItems(dashItems);
        }
    }
}
