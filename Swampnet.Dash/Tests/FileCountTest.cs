using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;

namespace Swampnet.Dash.Tests
{
	class FileCountTest : TestBase
	{
		protected override Task<ElementState> RunAsync()
		{
			var rs = new ElementState();

			var sw = Stopwatch.StartNew();

			var files = Directory.GetFiles(Definition.Parameters.StringValue("path"), Definition.Parameters.StringValue("filter", "*.*"));

			rs.Output.Add(new Property("count", files.Count()));
			rs.Output.Add(new Property("execution-elapsed", sw.ElapsedMilliseconds));

			return Task.FromResult(rs);
		}

		public override TestMeta Meta => new TestMeta()
		{
			Type = GetType().Name,
			Description ="",
			Parameters = new[]
			{
				new Property("path", "path to monitor"),
				new Property("filter", "wildcard filter")
			},
			Output = new[]
			{
				new Property("count", "Number of files in directory"),
				new Property("execution-elapsed", "command execution time (ms)")
			}
		};
	}
}
