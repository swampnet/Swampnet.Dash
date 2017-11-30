using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Swampnet.Dash.Common.Entities;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Diagnostics;

namespace Swampnet.Dash.Tests
{
	class CurlTest : ITest
	{
		public async Task<TestResult> RunAsync(TestDefinition testDefinition)
		{
			var rs = new TestResult(testDefinition.Id);


			var uri = testDefinition.Parameters.StringValue("uri");

			var timer = Stopwatch.StartNew();

			using (HttpClient httpClient = new HttpClient())
			{
				var response = await httpClient.GetAsync(uri);
				var content = await response.Content.ReadAsStringAsync();
			}

			var time = timer.Elapsed;

			// @TODO: Figure out state
			rs.State = "todo";

			rs.Properties.Add(new Property("value", time));

			return rs;
		}
	}
}
