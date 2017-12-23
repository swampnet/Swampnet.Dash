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
	public class ItemDefinition
	{
		[XmlAttribute]
		public string Id { get; set; }

		public List<Meta> MetaData { get; set; }

		public Plot Plot { get; set; }
	}


	public class Plot
	{
		[XmlAttribute]
		public ResolutionType Resolution { get; set; }

		[XmlIgnore]
		public TimeSpan History { get; set; }

		[XmlAttribute]
		public int MinY { get; set; }

		[XmlAttribute]
		public int MaxY { get; set; }

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

	public enum ResolutionType
	{
		Seconds,
		Minutes,
		Hours
	}
}
