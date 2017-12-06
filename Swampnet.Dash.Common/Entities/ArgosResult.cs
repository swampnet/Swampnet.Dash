using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class ArgosResult
    {
        public string ArgosId { get; set; }
        public IEnumerable<DashboardItem> Items { get; set; }
    }
}
