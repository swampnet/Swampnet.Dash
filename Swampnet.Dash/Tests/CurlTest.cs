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
	class CurlTest : TestBase
	{
        protected override async Task<ElementState> RunAsync()
        {
            var uri = Definition.Parameters.StringValue("uri");

            var timer = Stopwatch.StartNew();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();
            }

            var time = timer.Elapsed.TotalMilliseconds;

			var rs = new ElementState();

            rs.Output.Add(new Property("value", time));

            return rs;
        }


		public override TestMeta Meta => new TestMeta()
		{
			Type = GetType().Name,
			Description = "Get the response from an Http GET request",
			Parameters = new []
			{
				new Property(Constants.MANDATORY_CATEGORY, "uri", "Request Uri")
			},
			Output = new[]
			{
				new Property("value", "Response time (ms)")
			}
		};
	}
}
