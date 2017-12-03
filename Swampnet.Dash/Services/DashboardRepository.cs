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

        public async Task<DashMetaData> GetMetaDataAsync(string name)
        {           
			var dashboards = await GetActiveDashboardsAsync();
			var dash = dashboards.Single(d => d.Name == name);
			var tests = _testRepo.GetTestDefinitions();

			var meta = new DashMetaData();
			meta.Name = dash.Name;
			meta.Description = dash.Description;

			foreach (var testName in dash.Tests)
            {
                var test = tests.Single(t => t.Name == testName);
                var md = new DashItemMeta();
                md.Id = test.Name;
                md.Name = test.Name;
                md.Meta = test.MetaData;
                meta.Items.Add(md);
            }

            return meta;
        }


        public Task<IEnumerable<Dashboard>> GetActiveDashboardsAsync()
        {
            return Task.FromResult(Mock.Dashboards);
        }
    }
}
