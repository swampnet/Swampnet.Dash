using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IDashboardItemHistory
    {
        void AddTestResult(Element definition, ElementState result);
		ElementState GetCurrentState(Element definition);
        IEnumerable<ElementState> GetHistory(Element definition);
    }
}
