using Swampnet.Dash.Common;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
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
        public async Task<DashMetaData> GetMetaDataAsync(string name)
        {           
            var meta = new DashMetaData();
			var dashboards = await GetActiveDashboardsAsync();
			var dash = dashboards.Single(d => d.Name == name);

            foreach (var testName in dash.Tests)
            {
                var test = Mock.Tests.Single(t => t.Name == testName);
                var md = new DashItemMeta();
                md.Id = $"T-{test.Id}";
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
