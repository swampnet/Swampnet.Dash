using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class Dashboard
    {
        public Dashboard()
        {
			TestIds = new List<int>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> TestIds { get; set; }
    }
}
