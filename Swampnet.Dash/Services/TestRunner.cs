using Autofac;
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
		private IEnumerable<ITest> _tests;
		private readonly ITestRepository _testRepository;
        private readonly Dictionary<string, ElementState> _state = new Dictionary<string, ElementState>();
        private readonly IRuleProcessor _ruleProcessor;
		private readonly IValuesRepository _valuesRepository;
		private readonly ILifetimeScope _scope;

		public TestRunner(ITestRepository testRepo, IRuleProcessor ruleProcessor, IValuesRepository valuesRepository, ILifetimeScope scope)
        {
            _testRepository = testRepo;
            _ruleProcessor = ruleProcessor;
			_valuesRepository = valuesRepository;
			_scope = scope;
		}

        private IEnumerable<ITest> Tests
        {
            get
            {
                if(_tests == null)
                {
                    var tests = new List<ITest>();
                    foreach(var definition in _testRepository.GetDefinitions())
                    {
						var x = Type.GetType(definition.Type);
						if(x != null)
						{
							var test = _scope.Resolve(x) as ITest;
							if(test != null)
							{
								test.Configure(definition);
								tests.Add(test);
							}
						}
						// resolve instance of definition.Type
						// call .Configure(definition)
						// Add to tests
					}
					_tests = tests;
                }
                return _tests;
            }
        }

        public async Task<IEnumerable<ElementState>> RunAsync()
        {
            var results = new List<ElementState>();

			//Parallel.ForEach(_tests.Where(t => t.IsDue), test => { });
			foreach (var test in Tests.Where(t => t.IsDue))
			{
				try
				{
					var result = await test.RunAsync();
					result.Id = test.Id;    // nah, TestBase can do this
					lock (results)
					{
						results.Add(result);
					}

					await _ruleProcessor.ProcessTestResultAsync(test.Definition, result);

					Log.Debug("Test {type} - {name} = {value} / {status}", test.GetType().Name, test.Id, result.Output.StringValue("value"), result.Status);
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
			await _valuesRepository.Add(results);

			return results;
        }


		public IEnumerable<ElementState> GetTestResults(IEnumerable<string> ids)
		{
			return _state.Where(x => ids.Contains(x.Key)).Select(x => x.Value);
		}


		// Validate parameters against the test metadata
		//      private void Validate(Element testdefinition, TestMeta meta)
		//{
		//	foreach(var parameter in meta.Parameters)
		//	{
		//		if(testdefinition.Parameters.StringValue(parameter.Name, "") == "")
		//		{
		//			throw new ArgumentNullException($"Missing parameter '{parameter.Name}'");
		//		}
		//	}
		//}


		//public IEnumerable<Element> GetDue()
		//{
		//    var definitions = new List<Element>();

		//    foreach (var definition in _testRepository.GetDefinitions())
		//    {
		//        // Never been run
		//        if (!_state.ContainsKey(definition.Id))
		//        {
		//            definitions.Add(definition);
		//        }
		//        else
		//        {
		//            if (_state[definition.Id].TimestampUtc.Add(definition.Heartbeat) < DateTime.UtcNow)
		//            {
		//                definitions.Add(definition);
		//            }
		//        }
		//    }

		//    return definitions;
		//}

	}
}
