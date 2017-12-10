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
        private readonly ITestRepository _testRepository;
        private readonly IEnumerable<ITest> _tests;
        private readonly Dictionary<string, TestResult> _state = new Dictionary<string, TestResult>();


        public TestRunner(ITestRepository testRepo, IEnumerable<ITest> tests)
        {
            _testRepository = testRepo;
            _tests = tests;
        }


        public async Task<IEnumerable<TestResult>> RunAsync()
        {
            var results = new List<TestResult>();

            //Parallel.ForEach(GetDue(), testDefinition => { });
			foreach(var definition in GetDue())
            {
                try
                {
                    var test = _tests.Single(t => t.GetType().Name == definition.Type);

					Validate(definition, test.Meta);

                    Log.Debug("Running test {type} - {name}", test.GetType().Name, definition.Id);

                    var rs = await test.RunAsync(definition);

                    rs.TestId = definition.Id;

                    lock (results)
                    {
                        results.Add(rs);
                    }

                    //Log.Information("{test} '{id}' " + rs,
                    //    test.GetType().Name,
                    //    definition.Id);
                }
                catch (Exception ex)
                {
					Log.Error(ex, ex.Message);
                }
            }


            // Save state
            foreach (var testResult in results)
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

			// @TODO: So, if we're actually writing stuff away to the db as part of this, then it might take a while: We might be
			//        better off queing it up for later, and doing some kind of batch insert.
			_testRepository.Add(results);

			return results;
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


        public IEnumerable<TestDefinition> GetDue()
        {
            var definitions = new List<TestDefinition>();

            foreach (var definition in _testRepository.GetDefinitions())
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
