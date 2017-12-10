using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class Varient
    {
		public Varient(DateTime timestamp, string value)
		{
			Timestamp = timestamp;
			Value = value;
		}

		public DateTime Timestamp { get; set; }
		public string Value { get; set; }
	}
}
