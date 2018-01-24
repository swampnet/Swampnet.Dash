using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Rules
{
    public class ActionDefinition
    {
		// 'email'
		[XmlAttribute]
		public string ActionName { get; set; }

		// 'to:pj@theswamp.co.uk'
		public List<KeyValuePair<string, object>> Parameters { get; set; }

		public int? ConsecutiveHits { get; set; }
	}
}
