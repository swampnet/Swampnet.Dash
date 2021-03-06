﻿using Microsoft.AspNet.SignalR;
using Serilog;
using Swampnet.Dash.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Service.Hubs
{
    /// <summary>
    /// Client - callable stuff in here
    /// </summary>
    public class DashboardHub : Hub
    {
        public DashboardHub()
        {
        }

		// Just a test, we don't use this for anything
        public void Send(string name, string message)
        {
            Log.Debug($"{Context.ConnectionId} send '{message}'");

            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        /// <summary>
        /// Client is joining a group. A group == a dashboard, and a dashboard is really just a collection of DashItems
        /// </summary>
        /// <param name="groupName"></param>
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
            Log.Debug($"{Context.ConnectionId} joined group {groupName}");
        }


		public override Task OnConnected()
		{
			Log.Debug($"{Context.ConnectionId} Connected");
			return base.OnConnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			Log.Debug($"{Context.ConnectionId} disconnected");
			return base.OnDisconnected(stopCalled);
		}

		public override Task OnReconnected()
		{
			Log.Debug($"{Context.ConnectionId} reconnected");
			return base.OnReconnected();
		}
	}
}
