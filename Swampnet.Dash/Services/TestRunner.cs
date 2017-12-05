using Serilog;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

			foreach(var testdefinition in _testRepo.GetPendingTestDefinitions())
            {
                try
                {
                    var test = _tests.Single(t => t.GetType().Name == testdefinition.Type);

					Validate(testdefinition, test.Meta);

                    var rs = await test.RunAsync(testdefinition);
                    rs.TestId = testdefinition.Id;

					testResults.Add(rs);

					Log.Information("{test} '{id}' " + rs,
                        test.GetType().Name,
                        testdefinition.Id);

                    _testRepo.UpdateLastRun(testdefinition);
                }
                catch (Exception ex)
                {
					Log.Error(ex, ex.Message);

					testResults.Add(new TestResult() {
						State = "error",
						TestId = testdefinition.Id,
						TimestampUtc = DateTime.UtcNow
					});
                }
            }

            return testResults;
        }


		// Validate parameters against the test metadata
		private void Validate(TestDefinition testdefinition, TestMeta meta)
		{
			foreach(var parameter in meta.Parameters)
			{
				if(testdefinition.Parameters.StringValue(parameter.Name, "") == "")
				{
					throw new ArgumentNullException($"Missing parameter '{parameter.Name}'");
				}
			}
		}
	}
}
