using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class TestDefinition
    {
        public TestDefinition()
        {
            Parameters = new List<Property>();
            MetaData = new List<Meta>();
            States = new List<State>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public TimeSpan Heartbeat { get; set; }
        public ICollection<Property> Parameters { get; set; }
        public ICollection<Meta> MetaData { get; set; }
        public ICollection<State> States { get; set; }
    }

    public class Meta
    {
        public Meta()
        {
        }

        public Meta(string name, string type, string region)
        {
            Name = name;
            Type = type;
            Region = region;
        }

        public string Name { get; set; }
        public string Type { get; set; }  // Possibly VarEnum?
        public string Region { get; set; }
    }


    public class State
    {
        public string Name { get; set; }
        public string Expression { get; set; } // @TODO: Not a string!
    }
}
