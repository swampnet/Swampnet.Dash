using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class Dashboard
    {
        public Dashboard()
        {
			Tests = new List<string>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tests { get; set; }
    }
}
