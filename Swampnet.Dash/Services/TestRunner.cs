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

        public Task<IEnumerable<DashItem>> RunAsync()
        {
            var updatedDashItems = new List<DashItem>();
            var testDefinitions = _testRepo.GetPendingTestDefinitions();

            Parallel.ForEach(testDefinitions, async test =>
            {
                try
                {
                    var testRunner = _tests.Single(t => t.GetType().Name == test.Type);
                    var rs = await testRunner.RunAsync(test);

                    lock (updatedDashItems)
                    {
                        updatedDashItems.Add(rs);
                    }

                    Log.Information("{test} '{name}' " + rs,
                        testRunner.GetType().Name,
                        test.Name);

                    _testRepo.UpdateLastRun(test);
                }
                catch (Exception ex)
                {
                    // @TODO: Even when we error we need to broadcast an update (with the item in an error state)
                    Log.Error(ex, ex.Message);
                }
            });

            return Task.FromResult(updatedDashItems.AsEnumerable());
        }
    }
}
