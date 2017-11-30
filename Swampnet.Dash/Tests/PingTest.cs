using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Swampnet.Dash.Common.Entities;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Swampnet.Dash.Tests
{
	class PingTest : ITest
	{
		public async Task<TestResult> RunAsync(TestDefinition testDefinition)
		{
			var rs = new TestResult(testDefinition.Id);

			Ping ping = new Ping();

			var host = testDefinition.Parameters.StringValue("host");

			var reply = await ping.SendPingAsync(host);
			var time = reply.RoundtripTime;

			// @TODO: Figure out state
			rs.State = "todo";

			rs.Properties.Add(new Property("value", time));

			return rs;
		}
	}
}
