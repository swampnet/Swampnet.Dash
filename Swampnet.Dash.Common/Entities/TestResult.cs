using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Common.Entities
{
    public class TestResult
    {

        public TestResult()
        {
            Properties = new List<Property>();
            TimestampUtc = DateTime.UtcNow;
        }

        public TestResult(int testDefinitionId)
            : this()
        {
            TestDefinitionId = testDefinitionId;
        }


        public int TestDefinitionId { get; set; }
        public string State { get; set; }
        public DateTime TimestampUtc { get; set; }
        public List<Property> Properties { get; set; }

        public override string ToString()
        {
            return $"{State} (" + string.Join(",", Properties.Select(p => $"{p.Name}: {p.Value}")) + ")";
        }
    }
}
