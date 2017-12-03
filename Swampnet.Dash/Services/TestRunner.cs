using Serilog;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Services
{
    class TestRunner : ITestRunner
    {
        private readonly ITestRepository _testRepo;
        private readonly IEnumerable<ITest> _tests;

        public TestRunner(ITestRepository testRepo, IEnumerable<ITest> tests)
        {
            _testRepo = testRepo;
            _tests = tests;
        }

        public async Task<IEnumerable<TestResult>> RunAsync()
        {
            var testResults = new List<TestResult>();
            var testDefinitions = _testRepo.GetPendingTestDefinitions();

			foreach(var test in testDefinitions)
            {
                try
                {
                    var testRunner = _tests.Single(t => t.GetType().Name == test.Type);
                    var rs = await testRunner.RunAsync(test);
                    rs.TestName = test.Name;

					testResults.Add(rs);

					Log.Information("{test} '{name}' " + rs,
                        testRunner.GetType().Name,
                        test.Name);

                    _testRepo.UpdateLastRun(test);
                }
                catch (Exception ex)
                {
					Log.Error(ex, ex.Message);

					testResults.Add(new TestResult() {
						State = "error",
						TestName = test.Name,
						TimestampUtc = DateTime.UtcNow
					});
                }
            }

            return testResults;
        }
    }
}
