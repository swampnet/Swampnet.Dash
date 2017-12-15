using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface ITestRepository
    {
        IEnumerable<DashboardItemDefinition> GetDefinitions();
		void Add(IEnumerable<DashboardItem> testResults);
		IEnumerable<Varient> Get(string testId, string property);
	}
}
