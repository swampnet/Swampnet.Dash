using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common;

namespace Swampnet.Dash.Services
{
    class TestRepository : ITestRepository
    {
		// [id => DateTime]
        private readonly Dictionary<string, DateTime> _runtimes = new Dictionary<string, DateTime>();

        public IEnumerable<TestDefinition> GetTestDefinitions()
        {
            return Mock.Tests;
        }


        public IEnumerable<TestDefinition> GetPendingTestDefinitions()
        {
            var testDefinitions = new List<TestDefinition>();

            foreach (var testDefinition in GetTestDefinitions())
            {
                DateTime lastRun;
                if (!_runtimes.ContainsKey(testDefinition.Id))
                {
                    _runtimes.Add(testDefinition.Id, DateTime.MinValue);
                }
                lastRun = _runtimes[testDefinition.Id];

                if (lastRun.Add(testDefinition.Heartbeat) < DateTime.UtcNow)
                {
                    testDefinitions.Add(testDefinition);
                }
            }

            return testDefinitions;
        }


        public void UpdateLastRun(TestDefinition test)
        {
            lock (_runtimes)
            {
                _runtimes[test.Id] = DateTime.UtcNow;
            }
        }
    }
}
