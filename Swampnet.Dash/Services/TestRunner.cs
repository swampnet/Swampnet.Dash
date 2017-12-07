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
        private readonly Dictionary<string, TestResult> _state = new Dictionary<string, TestResult>();


        public TestRunner(ITestRepository testRepo, IEnumerable<ITest> tests)
        {
            _testRepo = testRepo;
            _tests = tests;
        }


        public async Task<IEnumerable<TestResult>> RunAsync()
        {
            var testResults = new List<TestResult>();

			foreach(var testdefinition in GetDue())
            {
                try
                {
                    var test = _tests.Single(t => t.GetType().Name == testdefinition.Type);

					Validate(testdefinition, test.Meta);

                    Log.Debug("Running test {type}", test.GetType().Name);

                    var rs = await test.RunAsync(testdefinition);

                    rs.TestId = testdefinition.Id;

					testResults.Add(rs);

					Log.Information("{test} '{id}' " + rs,
                        test.GetType().Name,
                        testdefinition.Id);

                    await SaveTestResultsAsync(testResults);
                }
                catch (Exception ex)
                {
					Log.Error(ex, ex.Message);

					testResults.Add(new TestResult() {
						Status = "error",
						TestId = testdefinition.Id,
						TimestampUtc = DateTime.UtcNow
					});
                }
            }
        
            return testResults;
        }


        public IEnumerable<TestResult> GetTestResults(IEnumerable<string> ids)
        {
            return _state.Where(x => ids.Contains(x.Key)).Select(x => x.Value);
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

        private Task SaveTestResultsAsync(IEnumerable<TestResult> testResults)
        {
            foreach (var testResult in testResults)
            {
                lock (_state)
                {
                    if (_state.ContainsKey(testResult.TestId))
                    {
                        _state[testResult.TestId] = testResult;
                    }
                    else
                    {
                        _state.Add(testResult.TestId, testResult);
                    }
                }
            }

            return Task.CompletedTask;
        }


        public IEnumerable<TestDefinition> GetDue()
        {
            var definitions = new List<TestDefinition>();

            foreach (var definition in _testRepo.GetDefinitions())
            {
                // Never been run
                if (!_state.ContainsKey(definition.Id))
                {
                    definitions.Add(definition);
                }
                else
                {
                    if (_state[definition.Id].TimestampUtc.Add(definition.Heartbeat) < DateTime.UtcNow)
                    {
                        definitions.Add(definition);
                    }
                }
            }

            return definitions;
        }

    }
}
