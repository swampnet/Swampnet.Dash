using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface ITestRunner
    {
        Task<IEnumerable<DashboardItem>> RunAsync();
        IEnumerable<DashboardItem> GetTestResults(IEnumerable<string> ids);
    }
}
