using Swampnet.Dash.Common;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using Swampnet.Dash.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash
{
    class DashboardRepository : IDashboardRepository
    {
		private readonly ITestRepository _testRepo;

		public DashboardRepository(ITestRepository testRepo)
		{
			_testRepo = testRepo;
		}

		public async Task<Dashboard> GetDashboardAsync(string id)
		{
			var dashboards = await GetDashboardsAsync();
			return dashboards.SingleOrDefault(d => d.Id == id);
		}


		public Task<IEnumerable<Dashboard>> GetDashboardsAsync()
        {
            return Task.FromResult(Mock.Dashboards);
        }
    }
}
