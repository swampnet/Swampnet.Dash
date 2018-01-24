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
		public List<ActionDefinitionParameter> Parameters { get; set; }

		public int? ConsecutiveHits { get; set; }
	}

	[XmlType("Parameter")]
	public class ActionDefinitionParameter
	{
		public ActionDefinitionParameter()
		{
		}

		public ActionDefinitionParameter(string name, string value)
			: this()
		{
			Name = name;
			Value = value;
		}

		[XmlAttribute]
		public string Name { get; set; }

		[XmlText]
		public string Value { get; set; }
	}
}
