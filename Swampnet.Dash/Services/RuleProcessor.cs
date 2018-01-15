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
		// rule-id -> history
		private readonly Dictionary<long, List<Tuple<DateTime, bool>>> _results = new Dictionary<long, List<Tuple<DateTime, bool>>>();

		// rule-id -> state-id -> history
		private readonly Dictionary<long, Dictionary<string, List<Tuple<DateTime, bool>>>> _aTypeResults = new Dictionary<long, Dictionary<string, List<Tuple<DateTime, bool>>>>();

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
			// run all valid rules
			foreach (var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null && r.StateModifiers.Any()))
			{
				foreach(var state in states)
				{
					var result = _expressionEvaluator.Evaluate(rule.Expression, state);

					AddResult(rule, state, result);

					var status = GetStatus(rule, state.Id);

					// Only upgrade status
					if (status > state.Status)
					{
						state.Status = status;
					}
				}

				Cleanup(rule, states);
			}

			return Task.CompletedTask;
		}


		public Task ProcessAsync(Element definition, ElementState state)
		{
			// run all valid rules
			foreach(var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null && r.StateModifiers.Any()))
			{
				var result = _expressionEvaluator.Evaluate(rule.Expression, state);

				AddResult(rule, result);

				var status = GetStatus(rule, null);

				// Only upgrade status
				if(status > state.Status)
				{
					state.Status = status;
				}
			}

			return Task.CompletedTask;
		}


		private Status GetStatus(Rule rule, string stateId)
		{
			var status = Status.Ok; // default to OK
			var history = string.IsNullOrEmpty(stateId)
				? GetRuleHistory(rule.Id).OrderByDescending(h => h.Item1)
				: GetRuleHistory(rule.Id, stateId).OrderByDescending(h => h.Item1);

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


		/// <summary>
		/// @TODO:
		/// rule => result is probably not good enough. It's ok for Tests where a rule is associated with exactly one State, but with
		/// a-types we run the same rule against multiple states
		/// </summary>
		/// <param name="rule"></param>
		/// <param name="result"></param>
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


		private void AddResult(Rule rule, ElementState state, bool result)
		{
			var ruleHistory = GetRuleHistory(rule.Id, state.Id);
			ruleHistory.Add(new Tuple<DateTime, bool>(DateTime.UtcNow, result));

			// trunc history
			var maxConsecutiveHist = rule.StateModifiers.Max(m => m.ConsecutiveHits);
			while (ruleHistory.Count > maxConsecutiveHist)
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


		private List<Tuple<DateTime, bool>> GetRuleHistory(long id, string stateId)
		{
			Dictionary<string, List<Tuple<DateTime, bool>>> lookup;

			if (_aTypeResults.ContainsKey(id))
			{
				lookup = _aTypeResults[id];
			}
			else
			{
				lookup = new Dictionary<string, List<Tuple<DateTime, bool>>>();
				_aTypeResults.Add(id, lookup);
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
		private void Cleanup(Rule rule, IEnumerable<ElementState> states)
		{
			//@todo: remove any history if it's not linked to a state
		}
	}
}
