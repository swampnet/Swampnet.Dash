using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common;

namespace Swampnet.Dash.Services
{
    class TestRepository : ITestRepository
    {
        public IEnumerable<TestDefinition> GetDefinitions()
        {
            return Mock.Tests;
        }
    }
}
