using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
	/// <summary>
	/// Define a single dashboard element
	/// </summary>
	/// <remarks>
	/// This can define:
	///		- A single test
	///		- A single 'argos' type structure (which may, and probably will, emit multiple Elements)
	/// </remarks>
    public class Element
    {
        public Element()
        {
            Parameters = new List<Property>();
            StateRules = new List<Rule>();
			Outputs = new List<Output>();
        }

		[XmlAttribute]
		public string Id { get; set; }

		[XmlAttribute]
		public string Description { get; set; }

		[XmlAttribute]
		public string Type { get; set; }

        [XmlIgnore]
		public TimeSpan Heartbeat { get; set; }


        // XmlSerializer does not support TimeSpan, so use this property for 
        // serialization instead.
        [Browsable(false)]
		[XmlAttribute(DataType = "duration", AttributeName = "Heartbeat")]
		public string __heartbeat
		{
			get
			{
				return XmlConvert.ToString(Heartbeat);
			}
			set
			{
				Heartbeat = string.IsNullOrEmpty(value) 
					? TimeSpan.Zero 
					: XmlConvert.ToTimeSpan(value);
			}
		}

        /// <summary>
        /// Test parameters
        /// </summary>
		public List<Property> Parameters { get; set; }

        /// <summary>
        /// Test Rules
        /// </summary>
        public List<Rule> StateRules { get; set; }

		/// <summary>
		/// Dynamic outputs
		/// </summary>
		public List<Output> Outputs { get; set; }
	}


	public enum OutputType
	{
		AVG
	}

	public class Output
	{
		[XmlAttribute]
		public OutputType Type { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		public Property[] Parameters { get; set; }
	}
}

