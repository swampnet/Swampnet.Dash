using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Common.Entities
{
    public class DashboardItem
    {
        public DashboardItem()
        {
            Output = new List<Property>();
            TimestampUtc = DateTime.UtcNow;
        }

        public DashboardItem(object id)
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

        /// <summary>
        /// Sorting hint
        /// </summary>
        public string Order { get; set; }

        // We probably need to return error state here as well? We need a way to signal that there was an error running the test.
        public string Status { get; set; }

        /// <summary>
        /// What is Timestamp? Is it the last time the item was updated, or the time it was initially created. We kinda need both under different
        /// circumstances. (We might want to display the last updated but, at least in an argos style dash, we probably want to order it by created)
        /// </summary>
        public DateTime TimestampUtc { get; set; }

        public List<Property> Output { get; set; }

        public override string ToString()
        {
            return $"{Status} (" + string.Join(",", Output.Select(p => $"{p.Name}: {p.Value}")) + ")";
        }
	}
}
