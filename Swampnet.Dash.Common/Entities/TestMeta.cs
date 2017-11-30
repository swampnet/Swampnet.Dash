using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class TestMeta
    {
		public TestMeta()
		{
		}

		public string Type { get; set; }
		public string Description { get; set; }
		public Property[] Parameters { get; set; }
		public Property[] Output { get; set; }
	}
}
