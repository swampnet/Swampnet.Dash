using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Common.Entities
{
    public class TestUpdate
    {

        public TestUpdate()
        {
            Properties = new List<Property>();
            TimestampUtc = DateTime.UtcNow;
        }

        public TestUpdate(int testDefinitionId)
            : this()
        {
            TestDefinitionId = testDefinitionId;
        }


        public int TestDefinitionId { get; set; }
        public string State { get; set; }
        public DateTime TimestampUtc { get; set; }
        public ICollection<Property> Properties { get; set; }

        public override string ToString()
        {
            return $"{State} (" + string.Join(",", Properties.Select(p => $"{p.Name}: {p.Value}")) + ")";
        }
    }
}
