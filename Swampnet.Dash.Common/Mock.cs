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
        public static Dashboard Dash(string name)
        {
            var dash = new Dashboard();
            dash.Id = name;
            dash.Description = name + " (description)";
            dash.Tests = Tests.Select(t => new ItemDefinition() { Id = t.Id }).ToList();
            return dash;
        }

        public static IEnumerable<TestDefinition> Tests
		{
			get
			{
				if(_tests == null)
				{
					_tests = File.ReadAllText("Data\\tests.xml").Deserialize<TestDefinition[]>();
				}
				return _tests;
			}
		}

        public static IEnumerable<ArgosDefinition> Argos
        {
            get
            {
                if (_argos == null)
                {
                    _argos = File.ReadAllText("Data\\argos.xml").Deserialize<ArgosDefinition[]>();
                }
                return _argos;
            }
        }

        public static IEnumerable<Dashboard> Dashboards
		{
			get
			{
				if(_dashboards == null)
				{
					_dashboards = File.ReadAllText("Data\\dashboards.xml").Deserialize<Dashboard[]>();
				}
				return _dashboards;
			}
		}

        private static IEnumerable<ArgosDefinition> _argos = null;
        private static IEnumerable<TestDefinition> _tests = null;
		private static IEnumerable<Dashboard> _dashboards = null;
	}
}
