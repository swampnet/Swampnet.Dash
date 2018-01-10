using Serilog;
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


		public async Task BroadcastState(ITest test)
		{
			// @todo: cache this, we don't want to be hitting the db each time
			var dashboards = await _dashRepository.GetDashboardsAsync();

			foreach(var dash in dashboards)
			{
				// If the dash references this test, broadcast an update
				if(dash.Tests.Any(t => t.Id == test.Id))
				{
					Log.Debug("Broadcast test {test_id} to dashboard {dash_id}", test.Id, dash.Id);

					_broadcast.Update(dash.Id, new[] { test.State });
				}
			}
		}
	}
}
