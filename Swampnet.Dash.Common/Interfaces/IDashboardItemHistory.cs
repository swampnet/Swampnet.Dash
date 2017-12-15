using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IDashboardItemHistory
    {
        void AddTestResult(DashboardItemDefinition definition, DashboardItem result);
		DashboardItem GetCurrentState(DashboardItemDefinition definition);
        IEnumerable<DashboardItem> GetHistory(DashboardItemDefinition definition);
    }
}
