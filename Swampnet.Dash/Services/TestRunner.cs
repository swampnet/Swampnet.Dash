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
        private readonly IRuleProcessor _ruleProcessor;
		private readonly IStateProcessor _stateProcessor;
		private readonly IValuesRepository _valuesRepository;
		private readonly ILifetimeScope _scope;

		public TestRunner(ITestRepository testRepo, IRuleProcessor ruleProcessor, IStateProcessor stateProcessor, IValuesRepository valuesRepository, ILifetimeScope scope)
        {
            _testRepository = testRepo;
            _ruleProcessor = ruleProcessor;
			_stateProcessor = stateProcessor;
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
						try
						{
							// @todo: Maybe pull this out into a TestFactory service, then you can keep your smelly service locator bullshit out of here!
							var x = Type.GetType(definition.Type);
							if (x != null)
							{
								var instance = _scope.Resolve(x) as ITest;
								if (instance != null)
								{
									instance.Configure(definition);
									tests.Add(instance);
								}
							}
						}
						catch (Exception ex)
						{
							Log.Error(ex, "Failed to create test '{test}'", definition.Id);
						}
					}
					_tests = tests;
                }
                return _tests;
            }
        }

        public async Task<IEnumerable<ElementState>> RunAsync()
        {
            var results = new List<ElementState>();

			//Parallel.ForEach(_tests.Where(t => t.IsDue), async test => { });
			foreach (var test in Tests.Where(t => t.IsDue))
			{
				try
				{
					// [pj] Right, here's our problem. We get a new test result each tim eso we loose our state.
					var result = await test.ExecuteAsync();

					lock (results)
					{
						results.Add(result);
					}

					await _stateProcessor.ProcessAsync(test.Definition, result);
					await _ruleProcessor.ProcessAsync(test.Definition, result);

					Log.Debug("Test {type} ({id}) {state}", 
						test.GetType().Name, 
						test.Id, 
						result);
				}
				catch (Exception ex)
				{
					Log.Error(ex, ex.Message);
				}
			}

			// @TODO: So, if we're actually writing stuff away to the db as part of this, then it might take a while: We might be
			//        better off queing it up for later, and doing some kind of batch insert.
			await _valuesRepository.Add(results);

			return results;
        }


		public IEnumerable<ElementState> GetTestResults(IEnumerable<string> ids)
		{
			return Tests.Where(t => ids.Contains(t.Id)).Select(t => t.State);
		}
	}
}
