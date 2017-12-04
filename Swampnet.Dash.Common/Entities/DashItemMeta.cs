using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
	/// <summary>
	/// Describes a DashItem from a client point of view (ie, stuff a client needs to render it)
	/// </summary>
    public class DashItemMeta
    {
        public DashItemMeta()
        {
            Meta = new List<Meta>();
        }

		/// <summary>
		/// This Id ties the meta data to a dashItem
		/// </summary>
		/// <remarks>
		/// This probably isn't good enough. Meta data -> dashItem isn't always a one-one relationship
		/// eg: 'Argos' style dash items probably all share the same meta-data.
		/// </remarks>
		[XmlAttribute]
		public string Id { get; set; }

		[XmlAttribute]
		public string Description { get; set; }

		public List<Meta> Meta { get; set; }
    }
}
