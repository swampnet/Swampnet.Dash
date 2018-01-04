﻿using Swampnet.Dash.Common.Interfaces;
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
        protected override Task<ElementState> Boosh()
        {
            var uri = Definition.Parameters.StringValue("uri");

            var timer = Stopwatch.StartNew();

            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();
            }

            var time = timer.Elapsed.TotalMilliseconds;

            // @TODO: Figure out state
            var rs = new ElementState()
            {
                Status = Status.Ok
            };

            rs.Output.Add(new Property("value", time));

            return rs;
        }


		public override TestMeta Meta
		{
			get
			{
				return new TestMeta()
				{
					Type = GetType().Name,
					Description = "Get the response from an Http GET request",
					Parameters = new []
					{
						new Property("uri", "Request Uri")
					},
					Output = new[]
					{
						new Property("value", "Response time (ms)")
					}
				};
			}
		}

	}
}
