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
		// rule-id -> state-id -> history
		private readonly Dictionary<long, Dictionary<string, List<Tuple<DateTime, bool>>>> _results = new Dictionary<long, Dictionary<string, List<Tuple<DateTime, bool>>>>();
		private readonly IExpressionEvaluator _expressionEvaluator;

		public RuleProcessor(IExpressionEvaluator expressionEvaluator)
        {
			_expressionEvaluator = expressionEvaluator;
		}

		/// <summary>
		/// Process multiple states as a single action - used for a-type
		/// </summary>
		/// <param name="definition"></param>
		/// <param name="states"></param>
		/// <returns></returns>
		public Task ProcessAsync(Element definition, IEnumerable<ElementState> states)
		{
			foreach (var state in states)
			{
				foreach (var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null && r.StateModifiers.Any()))
				{
					var result = _expressionEvaluator.Evaluate(rule.Expression, state);

					AddResult(rule, state.Id, result);

					state.Status = GetStatus(rule, state.Id, state.Status);
				}
			}

			// Remove any history we don't need any more
			Cleanup(states);

			return Task.CompletedTask;
		}


		public Task ProcessAsync(Element definition, ElementState state)
		{
			// run all valid rules
			foreach(var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null && r.StateModifiers.Any()))
			{
				var result = _expressionEvaluator.Evaluate(rule.Expression, state);

				AddResult(rule, state.Id, result);

				state.Status = GetStatus(rule, state.Id, state.Status);
			}

			return Task.CompletedTask;
		}


		private Status GetStatus(Rule rule, string stateId, Status currentStatus)
		{
			var status = Status.Ok; // default to OK
			var history = GetRuleHistory(rule.Id, stateId).OrderByDescending(h => h.Item1);

			foreach (var mod in rule.StateModifiers.OrderByDescending(m => m.ConsecutiveHits.HasValue ? m.ConsecutiveHits.Value : 0))
			{
				// Hit based
				if (!mod.ConsecutiveHits.HasValue || history.Count() >= mod.ConsecutiveHits.Value)
				{
					var range = history.Take(mod.ConsecutiveHits.HasValue ? Math.Max(mod.ConsecutiveHits.Value, 1) : 1); // Consecutive hits of 'zero' then. What does that mean?

					// All true. Use this modifier
					if (range.All(x => x.Item2))
					{
						status = mod.Value;
						break;
					}
				}
			}

			return status > currentStatus
				? status
				: currentStatus;
		}


		private void AddResult(Rule rule, string stateId, bool result)
		{
			var ruleHistory = GetRuleHistory(rule.Id, stateId);
			ruleHistory.Add(new Tuple<DateTime, bool>(DateTime.UtcNow, result));

			// trunc history
			var maxConsecutiveHist = rule.StateModifiers.Max(m => m.ConsecutiveHits);
			while (ruleHistory.Count > maxConsecutiveHist)
			{
				ruleHistory.RemoveAt(0);
			}
		}


		private List<Tuple<DateTime, bool>> GetRuleHistory(long id, string stateId)
		{
			Dictionary<string, List<Tuple<DateTime, bool>>> lookup;

			if (_results.ContainsKey(id))
			{
				lookup = _results[id];
			}
			else
			{
				lookup = new Dictionary<string, List<Tuple<DateTime, bool>>>();
				_results.Add(id, lookup);
			}

			List<Tuple<DateTime, bool>> history;

			if (lookup.ContainsKey(stateId))
			{
				history = lookup[stateId];
			}
			else
			{
				history = new List<Tuple<DateTime, bool>>();
				lookup.Add(stateId, history);
			}

			return history;
		}


		/// <summary>
		/// Remove any history if there are no instances of 'state'
		/// </summary>
		/// <param name="rule"></param>
		/// <param name="states"></param>
		private void Cleanup(IEnumerable<ElementState> states)
		{
			var stateIds = states.Select(x => x.Id);
			foreach(var lookup in _results.Values)
			{
				var expired = new List<string>();
				foreach(var stateId in lookup.Keys)
				{
					if (!stateIds.Contains(stateId))
					{
						expired.Add(stateId);
					}
				}

				foreach(var e in expired)
				{
					lookup.Remove(e);
				}
			}
		}
	}
}
