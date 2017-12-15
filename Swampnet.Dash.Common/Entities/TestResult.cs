using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Swampnet.Dash.Common.Interfaces;

namespace Swampnet.Dash.Common.Entities
{
    /// <summary>
    /// At the moment this is identical to DashItem - I just prefer the idea of teh test runner returning TestResults rather than DashItems!
    /// </summary>
    public class TestResult
    {
        public TestResult()
        {
            Output = new List<Property>();
            TimestampUtc = DateTime.UtcNow;
        }

        public string Id { get; set; }

        public Status Status { get; set; }

        public DateTime TimestampUtc { get; set; }

        public List<Property> Output { get; set; }

        public override string ToString()
        {
            return $"{Status} (" + string.Join(",", Output.Select(p => $"{p.Name}: {p.Value}")) + ")";
        }
    }
}
