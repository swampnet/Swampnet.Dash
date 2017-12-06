using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface ITestRunner
    {
        Task<IEnumerable<TestResult>> RunAsync();
        IEnumerable<TestResult> GetTestResults(IEnumerable<string> ids);
    }
}
