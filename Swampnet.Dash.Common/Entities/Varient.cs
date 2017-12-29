using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class Varient
    {
		public Varient(DateTime timestampUtc, double value)
		{
			TimestampUtc = timestampUtc;
			Value = value;
		}

		public DateTime TimestampUtc { get; set; }
		public double Value { get; set; }
	}
}
