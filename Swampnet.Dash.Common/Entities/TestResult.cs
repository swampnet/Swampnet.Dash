using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Common.Entities
{
    /// <summary>
    /// At the moment this is identical to DashItem - I just prefer the idea of teh test runner returning TestResults rather than DashItems!
    /// </summary>
    public class TestResult
    {
        public TestResult()
        {
            Properties = new List<Property>();
            TimestampUtc = DateTime.UtcNow;
        }

        public string TestName { get; set; }

        // We probably need to return error state here as well? We need a way to signal that there was an error running the test.
        public string State { get; set; }

        public DateTime TimestampUtc { get; set; }

        public List<Property> Properties { get; set; }

        public override string ToString()
        {
            return $"{State} (" + string.Join(",", Properties.Select(p => $"{p.Name}: {p.Value}")) + ")";
        }
    }
}
