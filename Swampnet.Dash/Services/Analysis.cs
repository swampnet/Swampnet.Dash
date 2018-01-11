using Serilog;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Services
{
	class Analysis : IAnalysis
	{
		private readonly Dictionary<string, List<ElementState>> _values = new Dictionary<string, List<ElementState>>();

		public Analysis()
		{

		}

		/// <summary>
		/// Q: Do we want to keep track in memory of this stuff, or hit up teh database?
		/// 
		/// - Really don't want to hit up the db each time
		/// - 
		/// </summary>
		public double Avg(ElementState state, string propertyName, TimeSpan history, bool isRolling)
		{
			List<ElementState> values = null;

			if (_values.ContainsKey(state.Id)) // think this should be ElementId rather than id...
			{
				values = _values[state.Id];
			}
			else
			{
				values = new List<ElementState>();
				_values.Add(state.Id, values);
			}

			values.Add(state);

			// @todo: trunc based on history (although, it's possible that we'll have multiple queries for the same test id with different history requirements...)

			var range = values.Where(s => s.TimestampUtc > DateTime.UtcNow.Subtract(history));
			var doubles = range.Select(s => s.Output.DoubleValue(propertyName));
			var avg = doubles.Any() ? doubles.Average() : 0.0;

			Log.Debug("{id} avg: {avg} / {count}", state.Id, avg, doubles.Count());

			return avg;
		}
	}
}
