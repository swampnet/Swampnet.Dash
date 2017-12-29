using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
	/// <summary>
	/// Maps a property
	/// </summary>
	/// <remarks>
	/// Includes stuff like the datatype so we can format it, the (string) format, and which region in the UI to display it.
	/// </remarks>
	public class Map
	{
		[XmlAttribute]
		public string Property { get; set; }

		[XmlAttribute]
		public string Constant { get; set; }

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
