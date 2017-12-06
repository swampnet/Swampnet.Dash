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
        {
            Name = name;
            Value = value.ToString();
        }

		[XmlAttribute]
		public string Category { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Value { get; set; }

		public override bool Equals(object obj)
		{
			var source = obj as Property;
			if(source == null)
			{
				return false;
			}

			return Category == source.Category
				&& Name == source.Name
				&& Value == source.Value;
		}

		// @TODO: This, properly.
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
