using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common;

namespace Swampnet.Dash.DAL
{
    class TestRepository : ITestRepository
    {
        public IEnumerable<ElementDefinition> GetDefinitions()
        {
            return Mock.Tests;
        }
	}
}
