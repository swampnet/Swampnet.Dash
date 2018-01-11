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
		private class ElementStateAnalysis
		{
			private TimeSpan _maxHistory = TimeSpan.MaxValue;
			private readonly List<ElementState> _values = new List<ElementState>();

			public string Id { get; private set; }

			public ElementStateAnalysis(string id)
			{
				Id = id;
			}


			public void Add(ElementState state)
			{
				_values.Add(state);
				Truncate();
			}

			public IEnumerable<double> GetValues(string propertyName, TimeSpan history)
			{
				if(_maxHistory < history || _maxHistory == TimeSpan.MaxValue)
				{
					_maxHistory = history;
				}

				var range = _values.Where(s => s.TimestampUtc > DateTime.UtcNow.Subtract(history));
				var doubles = range.Select(s => s.Output.DoubleValue(propertyName));

				return doubles;
			}


			private void Truncate()
			{
				// based on _maxHistory;
				if(_maxHistory != TimeSpan.MaxValue)
				{
					var range = _values.Where(s => s.TimestampUtc < DateTime.UtcNow.Subtract(_maxHistory)).ToList();
					foreach (var r in range)
					{
						_values.Remove(r);
					}
				}
			}
		}

		private readonly Dictionary<string, ElementStateAnalysis> _values = new Dictionary<string, ElementStateAnalysis>();

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
			var values = GetElementStateAnalysis(state.Id); // think this should be ElementId rather than id...

			values.Add(state);

			var doubles = values.GetValues(propertyName, history);
			var avg = doubles.Any() ? doubles.Average() : 0.0;

			Log.Debug("{id} avg: {avg} / {count}", state.Id, avg, doubles.Count());

			return avg;
		}

		private ElementStateAnalysis GetElementStateAnalysis(string id)
		{
			ElementStateAnalysis values = null;

			if (_values.ContainsKey(id))
			{
				values = _values[id];
			}
			else
			{
				values = new ElementStateAnalysis(id);
				_values.Add(id, values);
			}

			return values;
		}
	}
}
