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
        private readonly Dictionary<int, DateTime> _testRuntimes = new Dictionary<int, DateTime>();

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
                if (!_testRuntimes.ContainsKey(testDefinition.Id))
                {
                    _testRuntimes.Add(testDefinition.Id, DateTime.MinValue);
                }
                lastRun = _testRuntimes[testDefinition.Id];

                if (lastRun.Add(testDefinition.Heartbeat) < DateTime.UtcNow)
                {
                    testDefinitions.Add(testDefinition);
                }
            }

            return testDefinitions;
        }


        public void UpdateLastRun(TestDefinition test)
        {
            lock (_testRuntimes)
            {
                _testRuntimes[test.Id] = DateTime.UtcNow;
            }
        }
    }
}
