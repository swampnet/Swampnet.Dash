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
        private readonly Thread _runtimeThread;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ITestRunner _testRunner;

        public Runtime(
            ITestRunner testRunner,
            IDashboardRepository dashboardRepository)
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
                    var testResults = await _testRunner.RunAsync();

                    // Tests are independant of the dashboards they might appear in.
                    var dashboards = await _dashboardRepository.GetActiveDashboardsAsync();

                    foreach (var dash in dashboards)
                    {
                        string group = dash.Name;

                        // Get all the tests results referenced by tests in this dash:
                        var dashTestUpdates = testResults.Where(r => dash.Tests.Contains(r.TestName));
                        if (dashTestUpdates.Any())
                        {
                            var dashItems = dashTestUpdates.Select(tr => new DashItem()
                            {
                                Id = $"T:{tr.TestName}", // TODO: Need a better Id?
                                State = tr.State,
                                TimestampUtc = tr.TimestampUtc,
                                Properties = tr.Properties
                            });

                            Log.Debug("@TODO: Broadcast to '{group}' - {dashItems}", 
                                group,
                                string.Join(", ", dashItems.Select(di => di.Id)));
                        }
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
