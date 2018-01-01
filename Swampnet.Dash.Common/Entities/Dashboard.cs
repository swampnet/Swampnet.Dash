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

		[XmlAttribute]
		public int ElementsPerRow { get; set; }

		public List<ElementDefinition> Tests { get; set; }

        public List<ElementDefinition> Argos { get; set; }

        public List<Map> Mapping { get; set; }

        public List<DashboardGroup> Groups { get; set; }
    }
}
