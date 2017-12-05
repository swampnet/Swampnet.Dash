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
        private readonly IBroadcast _broadcast;
		private readonly IState _state;
		private readonly IArgosRunner _argosRunner;

		public Runtime(
            ITestRunner testRunner,
			IArgosRunner argosRunner,
			IState state,
            IDashboardRepository dashboardRepository,
            IBroadcast broadcast)
        {
			_state = state;
			_testRunner = testRunner;
			_argosRunner = argosRunner;
			_dashboardRepository = dashboardRepository;
			_broadcast = broadcast;

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
					// Run all the due tests
                    var testResults = await _testRunner.RunAsync();
					var argosResults = await _argosRunner.RunAsync();

                    var dashboards = await _dashboardRepository.GetDashboardsAsync();

                    foreach (var dash in dashboards)
                    {
						if(dash.Tests != null && dash.Tests.Any())
						{
							// Get all the tests results referenced by tests in this dash:
							var dashTestUpdates = testResults.Where(r => dash.Tests.Select(t => t.TestId).Contains(r.TestId));
							if (dashTestUpdates.Any())
							{
								var dashItems = dashTestUpdates.Select(tr => new DashboardItem()
								{
									Id = tr.TestId,
									Status = tr.State,
									TimestampUtc = tr.TimestampUtc,
									Output = tr.Output
								});

								await _state.SaveDashItemsAsync(dashItems);

								_broadcast.DashboardItems(dash.Id, dashItems);
							}
						}

						if(dash.Id == "argos-test")
						{
							_broadcast.DashboardItems(dash.Id, argosResults);
							await _state.SaveDashItemsAsync(argosResults);
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
