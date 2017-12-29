using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
    public class DashboardGroup
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public bool IsDefault { get; set; }

		/// <summary>
		/// @TODO: Group level mapping (NotImplemented)
		/// </summary>
		public List<Map> Mapping { get; set; }
	}
}
