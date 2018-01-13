using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Services
{
	class StateProcessor : IStateProcessor
	{
		private readonly IAnalysis _analysis;

		public StateProcessor(IAnalysis analysis)
		{
			_analysis = analysis;
		}


		public Task ProcessAsync(Element definition, ElementState state)
		{
			if(definition.Outputs != null)
			{
				ProcessAvg(state, definition.Outputs.Where(o => o.Type == OutputType.AVG));
				ProcessLast(state, definition.Outputs.Where(o => o.Type == OutputType.LAST));
			}

			return Task.CompletedTask;
		}


		private void ProcessAvg(ElementState state, IEnumerable<Output> outputs)
		{
			foreach(var output in outputs)
			{
				var source = output.Parameters.StringValue("source-property");
				var ts = output.Parameters.TimeSpanValue("timespan");

				var result = _analysis.Avg(state, source, ts, true);

				state.Output.AddOrUpdate(output.Name, result);
			}
		}

		/// <summary>
		/// 'Last value' stuff
		/// </summary>
		/// <param name="state"></param>
		/// <param name="outputs"></param>
		private void ProcessLast(ElementState state, IEnumerable<Output> outputs)
		{
			foreach (var output in outputs)
			{
				var source = output.Parameters.StringValue("source-property");
				var result = _analysis.Last(state, source);

				if(result != null)
				{
					state.Output.AddOrUpdate(output.Name, result);
				}
			}
		}

	}
}
