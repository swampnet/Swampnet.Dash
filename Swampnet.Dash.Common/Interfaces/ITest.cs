using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface ITest
    {
		Task<DashboardItem> RunAsync(TestDefinition testDefinition);
		TestMeta Meta { get; }
	}
}
