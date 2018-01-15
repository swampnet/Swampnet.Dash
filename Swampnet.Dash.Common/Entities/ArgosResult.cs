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
			var summary = new StringBuilder($"[{ArgosId}] ");
			foreach(var item in Items)
			{
				summary.Append($"[id:{item.Id} {item.Status}");
				summary.Append(string.Join(", ", item.Output.OrderBy(o => o.Name).Select(o => $"{o.Name}={o.Value}")));
				summary.Append("] ");
			}
			return summary.ToString().Trim();
        }
    }
}
