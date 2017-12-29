using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
	/// <summary>
	/// Define test id and any property mapping that goes with it
	/// </summary>
	public class ElementDefinition
	{
		[XmlAttribute]
		public string Id { get; set; }

		/// <summary>
		/// Element level meta data
		/// </summary>
		public List<Map> Mapping { get; set; }

		/// <summary>
		/// Plot/Graph info
		/// </summary>
		public Plot Plot { get; set; }
	}


	/// <summary>
	/// Define a Time based plot.
	/// </summary>
	public class Plot
	{
		/// <summary>
		/// How much history to keep
		/// </summary>
		[XmlIgnore]
		public TimeSpan History { get; set; }

		/// <summary>
		/// Minimum Y value
		/// </summary>
		[XmlAttribute]
		public double MinY { get; set; }

		/// <summary>
		/// Maximum Y value
		/// </summary>
		[XmlAttribute]
		public double MaxY { get; set; }

		/// <summary>
		/// The Output property used for Y
		/// </summary>
		[XmlAttribute]
		public string Output { get; set; }

		// XmlSerializer does not support TimeSpan, so use this property for 
		// serialization instead.
		[Browsable(false)]
		[XmlAttribute(DataType = "duration", AttributeName = "History")]
		public string __history
		{
			get
			{
				return XmlConvert.ToString(History);
			}
			set
			{
				History = string.IsNullOrEmpty(value)
					? TimeSpan.Zero
					: XmlConvert.ToTimeSpan(value);
			}
		}
	}
}
