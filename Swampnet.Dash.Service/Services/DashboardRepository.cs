using Swampnet.Dash.Common;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Service.Services
{
    public interface IDashboardRepository
    {
        ClientMeta GetMetaData(string dashId);
    }

    class DashboardRepository : IDashboardRepository
    {
        public ClientMeta GetMetaData(string dashId)
        {
            var dash = Mock.Dash();

            var meta = new ClientMeta();

            meta.Id = dashId;

            foreach (var id in dash.TestIds)
            {
                var test = Mock.Tests.Single(t => t.Id == id);
                var md = new ClientItemMeta();
                md.Id = id;
                md.Name = test.Name;
                md.Meta = test.MetaData;
                meta.Items.Add(md);
            }

            return meta;
        }
    }
}
