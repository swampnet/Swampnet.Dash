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
	/// </summary>
	/// <remarks>
	/// @TODO:
	/// - 'time' based 'consecutive hits'
	/// - 'value doesn't change for x hits'
	/// - 'Value > last value for x hits'
	/// - std deviation stuff
	/// </remarks>
	class RuleProcessor : IRuleProcessor
    {
		// rule-id -> List ( timestamp, result )
		private readonly Dictionary<long, List<Tuple<DateTime, bool>>> _results = new Dictionary<long, List<Tuple<DateTime, bool>>>();
		private readonly IExpressionEvaluator _expressionEvaluator;

		public RuleProcessor(IExpressionEvaluator expressionEvaluator)
        {
			_expressionEvaluator = expressionEvaluator;
		}


		public Task ProcessAsync(Element definition, ElementState state)
		{
			// run all valid rules
			foreach(var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null && r.StateModifiers.Any()))
			{
				var result = _expressionEvaluator.Evaluate(rule.Expression, state);

				AddResult(rule, result);

				var status = GetStatus(rule);

				// Only upgrade status
				if(status > state.Status)
				{
					state.Status = status;
				}
			}

			return Task.CompletedTask;
		}


		private Status GetStatus(Rule rule)
		{
			var status = Status.Ok; // default to OK
			var history = GetRuleHistory(rule.Id).OrderByDescending(h => h.Item1);

			foreach (var mod in rule.StateModifiers.OrderByDescending(m => m.ConsecutiveHits.HasValue ? m.ConsecutiveHits.Value : 0))
			{
				// Hit based
				if (!mod.ConsecutiveHits.HasValue || history.Count() >= mod.ConsecutiveHits.Value)
				{
					var range = history.Take(mod.ConsecutiveHits.HasValue ? Math.Max(mod.ConsecutiveHits.Value, 1) : 1); // Consecutive hits of 'zero' then. What does that mean?
					//Log.Debug($"Rule: [{rule.Id}] {mod.ConsecutiveHits} - " + string.Join(",", range.Select(x => x.Item2)));
					// All true. Use this modifier
					if (range.All(x => x.Item2))
					{
						status = mod.Value;
						break;
					}
				}
			}

			return status;
		}


		private void AddResult(Rule rule, bool result)
		{
			var ruleHistory = GetRuleHistory(rule.Id);
			ruleHistory.Add(new Tuple<DateTime, bool>(DateTime.UtcNow, result));

			// trunc history
			var maxConsecutiveHist = rule.StateModifiers.Max(m => m.ConsecutiveHits);
			while(ruleHistory.Count > maxConsecutiveHist)
			{
				ruleHistory.RemoveAt(0);
			}
		}


		private List<Tuple<DateTime, bool>> GetRuleHistory(long id)
		{
			List<Tuple<DateTime, bool>> history;

			if (_results.ContainsKey(id))
			{
				history = _results[id];
			}
			else
			{
				history = new List<Tuple<DateTime, bool>>();
				_results.Add(id, history);
			}

			return history;
		}
    }
}
