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
		public string Name { get; set; }

		public List<Meta> Meta { get; set; }
    }


	/// <summary>
	/// Maps a property
	/// </summary>
	/// <remarks>
	/// Includes stuff like the datatype so we can format it, the (string) format, and which region in the UI to display it.
	/// </remarks>
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

		/// <summary>
		/// .. as in string.Format()
		/// </summary>
		[XmlAttribute]
		public string Format { get; set; }

		[XmlAttribute]
		public string Region { get; set; }
	}
}
