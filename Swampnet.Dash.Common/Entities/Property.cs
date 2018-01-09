using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
    public class Property
    {
        public Property()
        {
        }

        public Property(string name, object value)
			: this()
        {
            Name = name;
            Value = value.ToString();
        }

		public Property(string category, string name, object value)
			: this(name, value)
		{
			Category = category;
		}

		[XmlAttribute]
		public string Category { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		//[XmlAttribute]
		[XmlText]
		public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}
