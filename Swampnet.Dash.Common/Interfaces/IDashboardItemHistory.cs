using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IDashboardItemHistory
    {
        void AddTestResult(TestDefinition definition, DashboardItem result);
		DashboardItem GetCurrentState(TestDefinition definition);
        IEnumerable<DashboardItem> GetHistory(TestDefinition definition);
    }
}
