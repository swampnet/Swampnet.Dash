using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Swampnet.Dash.Common
{
    public static class Mock
    {
        public static Dashboard Dash()
        {
            var dash = new Dashboard();
            dash.Id = Guid.NewGuid();
            dash.Name = "Pings";
            dash.Description = "All the tests";
            dash.TestIds = Tests.Select(t => t.Id).ToList();
            return dash;
        }

        public static IEnumerable<TestDefinition> Tests
		{
			get
			{
				if(_tests == null)
				{
					_tests = File.ReadAllText("Data\\TestDefinitions.xml").Deserialize<TestDefinition[]>();
				}
				return _tests;
			}
		}


		private static IEnumerable<TestDefinition> _tests = null;
    }
}
