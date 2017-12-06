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
		private readonly IArgosRunner _argosRunner;

		public Runtime(
            ITestRunner testRunner,
			IArgosRunner argosRunner,
            IDashboardRepository dashboardRepository,
            IBroadcast broadcast)
        {
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

                    // Get latest Argos results. Note, this will probably be *all* the dashItems from *all* the argos style dashes
					var argosResults = await _argosRunner.RunAsync();

                    var dashboards = await _dashboardRepository.GetDashboardsAsync();

                    foreach (var dash in dashboards)
                    {
                        // Handle any tests if there are any
						if(dash.Tests != null && dash.Tests.Any())
						{
							// Get all the tests results referenced by tests in this dash:
							var dashTestUpdates = testResults.Where(r => dash.Tests.Select(t => t.Id).Contains(r.TestId));
							if (dashTestUpdates.Any())
							{
								var dashItems = dashTestUpdates.Select(tr => new DashboardItem()
								{
									Id = tr.TestId,
									Status = tr.Status,
									TimestampUtc = tr.TimestampUtc,
									Output = tr.Output
								});

								_broadcast.Update(dash.Id, dashItems);
							}
						}

                        // Handle Argos stuff
                        if(dash.Argos != null && dash.Argos.Any())
                        {
                            // We collate *all* the 'argos' stuff into one single per-dash update.
                            var broadcast = new List<DashboardItem>();
                            
                            foreach(var rs in argosResults.Where(a => dash.Argos.Select(b => b.Id).Contains(a.ArgosId)))
                            {
                                broadcast.AddRange(rs.Items);
                            }

                            if (broadcast.Any())
                            {
                                _broadcast.Refresh(dash.Id, broadcast);
                            }
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
