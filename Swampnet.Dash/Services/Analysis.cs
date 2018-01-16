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
	/// <summary>
	/// Pretty sure we can move all this into StateProcessor, although I guess it does make sense for this to be it's own thing...
	/// </summary>
	class Analysis : IAnalysis
	{
		private class ElementStateAnalysis
		{
			public string Id { get; private set; }
			private TimeSpan _maxHistory = TimeSpan.MaxValue;
			private readonly List<ElementState> _values = new List<ElementState>();

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
			var values = GetElementStateAnalysis(state.Id);

			values.Add(state);

			var doubles = values.GetValues(propertyName, history);
			var avg = doubles.Any() ? doubles.Average() : 0.0;

			return avg;
		}

		private readonly Dictionary<string, ElementState> _lastState = new Dictionary<string, ElementState>();

		/// <summary>
		/// Get last value
		/// </summary>
		/// <param name="state"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public object Last(ElementState state, string propertyName)
		{
			object value = null;

			if (_lastState.ContainsKey(state.Id))
			{
				var last = _lastState[state.Id];
				value = last.Output.Value(propertyName);
			}

			_lastState[state.Id] = state.Copy();

			return value;
		}



		public double StdDev(ElementState state, string propertyName, TimeSpan history, bool isSample)
		{
			var values = GetElementStateAnalysis(state.Id);

			values.Add(state);

			var doubles = values.GetValues(propertyName, history);

			double mean = 0.0;
			double sum = 0.0;
			double stdDev = 0.0;
			int n = 0;

			foreach (double val in doubles)
			{
				n++;
				double delta = val - mean;
				mean += delta / n;
				sum += delta * (val - mean);
			}

			if (1 < n)
			{
				stdDev = isSample
					? Math.Sqrt(sum / (n - 1))
					: Math.Sqrt(sum / (n));
			}

			return stdDev;
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
