using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
	/// <summary>
	/// Dashboard definition
	/// </summary>
	/// <remarks>
	/// Contains configuration and meta data for a single dashboard. Any tests are defined elsware
	/// </remarks>
    public class Dashboard
    {
		[XmlAttribute]
        public string Id { get; set; }
		   
		[XmlAttribute]
		public string Description { get; set; }

        [XmlAttribute]
        public string Template { get; set; }

        public List<ItemDefinition> Tests { get; set; }

        public List<ItemDefinition> Argos { get; set; }

        public List<Meta> DefaultMetaData { get; set; }

        public List<DashboardGroup> Groups { get; set; }
    }
}
