﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;

namespace Swampnet.Dash.Tests
{
	class SqlCommandTest : TestBase
	{
		protected async override Task UpdateAsync()
		{
			var connectionName = Definition.Parameters.StringValue("connection-name");
			var query = Definition.Parameters.StringValue("query");
			var sw = Stopwatch.StartNew();

			using(var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString))
			{
				await connection.OpenAsync();

				using (var cmd = new SqlCommand(query, connection))
				{
					cmd.CommandTimeout = Definition.Parameters.IntValue("timeout", 30);
					var reader = await cmd.ExecuteReaderAsync();
					var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

					if (await reader.ReadAsync())
					{
						foreach(var column in columns)
						{
							State.Output.AddOrUpdate(column, reader[column]);
						}
					}
				}
			}

			State.Output.AddOrUpdate("execution-elapsed", sw.ElapsedMilliseconds);
		}


		public override TestMeta Meta => new TestMeta()
		{
			Type = GetType().Name,
			Description = "",
			Output = new[]
			{
				new Property("execution-elapsed", "command execution time (ms)")
			},
			Parameters = new[]
			{
				new Property(Constants.MANDATORY_CATEGORY, "connection-name", "Connection name, as defined in service configuration"),
				new Property(Constants.MANDATORY_CATEGORY, "query", "SQL query"),
				new Property(Constants.OPTIONAL_CATEGORY, "timeout", "SQL command timeout (s)")
			}
		};
	}
}
