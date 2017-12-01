using Swampnet.Dash.Common;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash
{
    class DashboardRepository : IDashboardRepository
    {
        public ClientMeta GetMetaData(string dashId)
        {
            var dash = Mock.Dash(dashId);

            var meta = new ClientMeta();

            meta.Id = dashId;

            foreach (var name in dash.Tests)
            {
                var test = Mock.Tests.Single(t => t.Name == name);
                var md = new ClientItemMeta();
                md.Id = $"T-{test.Id}";
                md.Name = test.Name;
                md.Meta = test.MetaData;
                meta.Items.Add(md);
            }

            return meta;
        }

        public Task<IEnumerable<Dashboard>> GetActiveDashboardsAsync()
        {
            var dashboards = new[] {
                Mock.Dash("dash-01"),
                Mock.Dash("dash-02")
            };

            return Task.FromResult(dashboards.AsEnumerable());
        }
    }
}
