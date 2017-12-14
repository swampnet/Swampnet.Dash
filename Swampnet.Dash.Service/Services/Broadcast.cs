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
        public void Update(string group, IEnumerable<DashboardItem> dashItems)
        {
            //Log.Debug("Update: '{group}' - {dashItems}",
            //    group,
            //    string.Join(", ", dashItems.Select(di => di.Id)));

            GlobalHost.ConnectionManager
                .GetHubContext<DashboardHub>()
                .Clients
                .Group(group)
                .Update(dashItems);
        }


        public void Refresh(string group, IEnumerable<DashboardItem> dashItems)
        {
            //Log.Debug("Refresh: '{group}' - {dashItems}",
            //    group,
            //    string.Join(", ", dashItems.Select(di => di.Id)));

            GlobalHost.ConnectionManager
                .GetHubContext<DashboardHub>()
                .Clients
                .Group(group)
                .Refresh(dashItems);
        }
    }
}
