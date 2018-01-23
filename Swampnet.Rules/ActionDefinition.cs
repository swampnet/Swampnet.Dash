using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Rules
{
    public class ActionDefinition
    {
		// 'email'
		public string ActionName { get; set; }

		// 'to:pj@theswamp.co.uk'
		public List<KeyValuePair<string, object>> Parameters { get; set; }

		public int? ConsecutiveHits { get; set; }
	}
}
