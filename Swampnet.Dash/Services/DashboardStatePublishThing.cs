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
	class DashboardStatePublishThing : IDashboardStatePublishThing
	{
		private readonly IDashboardRepository _dashRepository;
		private readonly IBroadcast _broadcast;

		public DashboardStatePublishThing(IDashboardRepository dashRepository, IBroadcast broadcast)
		{
			_dashRepository = dashRepository;
			_broadcast = broadcast;
		}


		/// <summary>
		/// Publish a test result
		/// </summary>
		/// <param name="test"></param>
		/// <returns></returns>
		public async Task BroadcastState(ITest test)
		{
			// @todo: cache this, we don't want to be hitting the db each time
			var dashboards = await _dashRepository.GetDashboardsAsync();

			foreach(var dash in dashboards)
			{
				// If the dash references this test, broadcast an update
				if(dash.Tests.Any(t => t.Id == test.Id))
				{
					Log.Debug("Broadcast test {test_id} to dashboard {dash_id} => {status}", test.Id, dash.Id, test.State.Status);

					_broadcast.Update(dash.Id, new[] { test.State });
				}
			}
		}


		public async Task BroadcastState(IEnumerable<ArgosResult> items)
		{
			// @todo: cache this, we don't want to be hitting the db each time
			var dashboards = await _dashRepository.GetDashboardsAsync();

			foreach (var dash in dashboards)
			{
				if (dash.Argos != null && dash.Argos.Any())
				{
					// We collate *all* the 'argos' stuff into one single per-dash update.
					var broadcast = new List<ElementState>();

					foreach (var rs in items.Where(a => dash.Argos.Select(b => b.Id).Contains(a.ArgosId)))
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
	}
}
