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
		protected override Task UpdateAsync()
		{
			var sw = Stopwatch.StartNew();

			var files = Directory.GetFiles(Definition.Parameters.StringValue("path"), Definition.Parameters.StringValue("filter", "*.*"));
			
			State.Output.AddOrUpdate("count", files.Count());
			State.Output.AddOrUpdate("execution-elapsed", sw.ElapsedMilliseconds);

			return Task.CompletedTask;
		}

		public override TestMeta Meta => new TestMeta()
		{
			Type = GetType().Name,
			Description ="",
			Parameters = new[]
			{
				new Property(Constants.MANDATORY_CATEGORY, "path", "path to monitor"),
				new Property(Constants.OPTIONAL_CATEGORY, "filter", "wildcard filter")
			},
			Output = new[]
			{
				new Property("count", "Number of files in directory"),
				new Property("execution-elapsed", "command execution time (ms)")
			}
		};
	}
}
