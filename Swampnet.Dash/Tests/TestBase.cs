using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Tests
{
    abstract class TestBase : ITest
    {
        private Element _testDefinition;
        private DateTime _lastRunUtc = DateTime.MinValue;

        public Element Definition => _testDefinition;

        public string Id => _testDefinition.Id;

        abstract public TestMeta Meta { get; }

        public Task<ElementState> RunAsync()
        {
            var rs = Boosh();

            _lastRunUtc = DateTime.UtcNow;

            return rs;
        }

        public void Configure(Element testDefinition)
        {
            _testDefinition = testDefinition;
        }

        public bool IsDue
        {
            get
            {
                return _testDefinition != null && ((DateTime.UtcNow - _lastRunUtc) > _testDefinition.Heartbeat);
            }
        }

        protected abstract Task<ElementState> Boosh();
    }
}
