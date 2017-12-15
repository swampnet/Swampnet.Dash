using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IDashboardItemHistory
    {
        void AddTestResult(TestDefinition definition, TestResult result);
        TestResult GetCurrentState(TestDefinition definition);
        IEnumerable<TestResult> GetHistory(TestDefinition definition);
    }
}
