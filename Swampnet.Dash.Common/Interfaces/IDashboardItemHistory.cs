using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IDashboardItemHistory
    {
        void AddTestResult(ElementDefinition definition, Element result);
		Element GetCurrentState(ElementDefinition definition);
        IEnumerable<Element> GetHistory(ElementDefinition definition);
    }
}
