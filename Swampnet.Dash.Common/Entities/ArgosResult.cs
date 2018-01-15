using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class ArgosResult
    {
        public string ArgosId { get; set; }
        public IEnumerable<ElementState> Items { get; set; }
        public DateTime TimestampUtc { get; private set; }

        public ArgosResult()
        {
            TimestampUtc = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"[{ArgosId}] --> " + string.Join(" ", Items.Select(i => $"[{i.Id}: ({i.Status})]"));
        }
    }
}
