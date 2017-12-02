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
        public string Id { get; set; }
        public List<DashItemMeta> Items { get; set; }
    }


	/// <summary>
	/// Describes a DashItem from a client point of view (ie, stuff a client needs to render it)
	/// </summary>
    public class DashItemMeta
    {
        public DashItemMeta()
        {
            Meta = new List<Meta>();
        }

		[XmlAttribute]
		public string Id { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		public List<Meta> Meta { get; set; }
    }


	/// <summary>
	/// Map a property
	/// </summary>
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

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Type { get; set; }  // Possibly VarEnum?

		[XmlAttribute]
		public string Region { get; set; }
	}
}
