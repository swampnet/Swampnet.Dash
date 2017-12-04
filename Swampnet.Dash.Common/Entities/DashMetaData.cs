using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
	/// <summary>
	/// Describes the dash from a client point of view (ie, stuff a client needs to render it)
	/// </summary>
    public class DashMetaData
    {
        public DashMetaData()
        {
            Items = new List<DashItemMeta>();
        }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Description { get; set; }

		public List<DashItemMeta> Items { get; set; }
    }
}
