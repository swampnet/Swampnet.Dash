using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Common.Entities
{
    public class DashItem
    {
        public DashItem()
        {
            Output = new List<Property>();
            TimestampUtc = DateTime.UtcNow;
        }

        public DashItem(object id)
            : this()
        {
            Id = id.ToString();
        }

        /// <summary>
        /// Identifies this item within a dashboard
        /// </summary>
        /// <remarks>
        /// This could be a test definition id, or something else. It kind of doesn't matter as long as
        /// the client can use it to find this item on an update
        /// </remarks>
        public string Id { get; set; }

        // We probably need to return error state here as well? We need a way to signal that there was an error running the test.
        public string State { get; set; }

        public DateTime TimestampUtc { get; set; }

        public List<Property> Output { get; set; }

        public override string ToString()
        {
            return $"{State} (" + string.Join(",", Output.Select(p => $"{p.Name}: {p.Value}")) + ")";
        }
    }
}
