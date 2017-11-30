using System;
using System.Collections.Generic;
using System.Text;

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
            Value = value;
        }

        public string Category { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
