using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
	/// <summary>
	/// Describes data about a test type (What parameters it requires, what you can expect the output to be)
	/// </summary>
    public class TestMeta
    {
		/// <summary>
		/// The CLR Type this data maps to
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// A description of the test. Generally this is for internal use.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Required parameters
		/// </summary>
		public Property[] Parameters { get; set; }

		/// <summary>
		/// The output values you can expect
		/// </summary>
		/// <remarks>
		/// Name: Property Name
		/// Value: Description of the output
		/// </remarks>
		public Property[] Output { get; set; }
	}
}
