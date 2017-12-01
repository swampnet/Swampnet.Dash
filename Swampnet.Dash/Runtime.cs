using Serilog;
using Swampnet.Dash.Common;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Swampnet.Dash
{
    public class Runtime : IRuntime
    {
        private readonly IEnumerable<ITest> _tests;
        private readonly Thread _runtimeThread;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestRunner _testRunner;

        public Runtime(
            ITestRunner testRunner,
            IDashboardRepository dashboardRepository
            )
        {
            _testRunner = testRunner;
            _dashboardRepository = dashboardRepository;

            _runtimeThread = new Thread(RuntimeThread);
            _runtimeThread.IsBackground = true;
        }


        public void Start()
        {
            _runtimeThread.Start();
        }

        public void Stop()
        {
            // HACK
            _runtimeThread.Abort();
        }

        private async void RuntimeThread()
        {
            while (true)
            {
                try
                {
                    var updatedDashItems = await _testRunner.RunAsync();

                    // Tests are independant of the dashboards they might appear in.
                    // @TODO: For each dashboard, fire off updates if they contain any of the tests we've just run...
                    var dashboards = await _dashboardRepository.GetActiveDashboardsAsync();

                    foreach (var dash in dashboards)
                    {
                        string groupName = dash.Name;
                    }
                }
				catch (ThreadAbortException)
				{
					break;
				}
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
