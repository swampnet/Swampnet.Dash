using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IState
    {
		Task<IEnumerable<DashboardItem>> GetDashItemsAsync(IEnumerable<string> ids);
		Task SaveDashItemsAsync(IEnumerable<DashboardItem> dashItems);
	}
}
