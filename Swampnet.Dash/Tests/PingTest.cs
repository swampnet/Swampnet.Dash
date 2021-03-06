﻿using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Swampnet.Dash.Common.Entities;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Swampnet.Dash.Tests
{
	class PingTest : TestBase
	{
        protected override async Task UpdateAsync()
        {
            var ping = new Ping();

            var host = Definition.Parameters.StringValue("host");

            var reply = await ping.SendPingAsync(host);
            var time = reply.RoundtripTime;

            var rs = new ElementState();

            State.Output.AddOrUpdate("value", time);
        }


		public override TestMeta Meta => new TestMeta()
		{
			Type = GetType().Name,
			Description = "",
			Parameters = new []
			{
				new Property(Constants.MANDATORY_CATEGORY, "host", "Hostname or address")
			},
			Output = new []
			{
				new	Property("value", "Ping response time (ms)")
			}
		};
	}
}
