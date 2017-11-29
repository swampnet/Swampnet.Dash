using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class DashItem
    {
        public DashItem()
        {
            Properties = new List<Property>();
            UpdatedUtc = DateTime.UtcNow;
        }


        public Guid Id { get; set; }
        public string State { get; set; }
        public ICollection<Property> Properties { get; set; }
        public DateTime UpdatedUtc { get; set; }

        // If we want to remove an item from a dash, we send it with enabled = false
        public bool IsEnabled { get; set; }
    }
}
