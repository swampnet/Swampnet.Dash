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
        private readonly Dictionary<string, Element> _state = new Dictionary<string, Element>();
        private readonly IRuleProcessor _ruleProcessor;

        public TestRunner(ITestRepository testRepo, IEnumerable<ITest> tests, IRuleProcessor ruleProcessor)
        {
            _testRepository = testRepo;
            _tests = tests;
            _ruleProcessor = ruleProcessor;
        }


        public async Task<IEnumerable<Element>> RunAsync()
        {
            var results = new List<Element>();

            //Parallel.ForEach(GetDue(), testDefinition => { });
			foreach(var definition in GetDue())
            {
                try
                {
                    var test = _tests.Single(t => t.GetType().Name == definition.Type);

					Validate(definition, test.Meta);

                    //Log.Debug("Running test {type} - {name}", test.GetType().Name, definition.Id);

                    var result = await test.RunAsync(definition);

					result.Id = definition.Id;

                    lock (results)
                    {
                        results.Add(result);
                    }

                    await _ruleProcessor.ProcessTestResultAsync(definition, result);

					Log.Debug("Test {type} - {name} = {value} / {status}", test.GetType().Name, definition.Id, result.Output.StringValue("value"), result.Status);
				}
				catch (Exception ex)
                {
					Log.Error(ex, ex.Message);
                }
            }


            // Save state
            foreach (var testResult in results)
            {
                if (_state.ContainsKey(testResult.Id))
                {
                    _state[testResult.Id] = testResult;
                }
                else
                {
                    _state.Add(testResult.Id, testResult);
                }

			}

			// @TODO: So, if we're actually writing stuff away to the db as part of this, then it might take a while: We might be
			//        better off queing it up for later, and doing some kind of batch insert.
			_testRepository.Add(results);

			return results;
        }


        public IEnumerable<Element> GetTestResults(IEnumerable<string> ids)
        {
            return _state.Where(x => ids.Contains(x.Key)).Select(x => x.Value);
        }


        // Validate parameters against the test metadata
        private void Validate(ElementDefinition testdefinition, TestMeta meta)
		{
			foreach(var parameter in meta.Parameters)
			{
				if(testdefinition.Parameters.StringValue(parameter.Name, "") == "")
				{
					throw new ArgumentNullException($"Missing parameter '{parameter.Name}'");
				}
			}
		}


        public IEnumerable<ElementDefinition> GetDue()
        {
            var definitions = new List<ElementDefinition>();

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
