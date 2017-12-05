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
        public Dashboard()
        {
			Tests = new List<TestItemDefinition>();
        }

		[XmlAttribute]
        public string Id { get; set; }

		[XmlAttribute]
		public string Description { get; set; }

        public List<TestItemDefinition> Tests { get; set; }

		public List<Meta> DefaultMetaData { get; set; }
    }


	/// <summary>
	/// Define test id and any property mapping that goes with it
	/// </summary>
	public class TestItemDefinition
	{
		[XmlAttribute]
		public string TestId { get; set; }

		public List<Meta> MetaData { get; set; }
	}
}
