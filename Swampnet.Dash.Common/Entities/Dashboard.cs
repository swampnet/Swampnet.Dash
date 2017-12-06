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

        public List<ItemDefinition> Tests { get; set; }

        public List<ItemDefinition> Argos { get; set; }

        public List<Meta> DefaultMetaData { get; set; }
    }


    /// <summary>
    /// Define test id and any property mapping that goes with it
    /// </summary>
    public class ItemDefinition
	{
		[XmlAttribute]
		public string Id { get; set; }

		public List<Meta> MetaData { get; set; }
	}
}
