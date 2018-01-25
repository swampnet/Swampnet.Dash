using Serilog;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Rules;

namespace Swampnet.Dash.Services
{
	class RuleProcessor : IRuleProcessor
	{
		public RuleProcessor()
		{
			// @TODO: Set up rules in a sensible way?
			Rule.ForAction("set-state").Execute((def, ctx) => {
				var state = ctx as ElementState;
				if (state != null)
				{
					var newState = (Status)Enum.Parse(typeof(Status), def.Parameters.Value("state", "Ok"));
					Log.Debug(">>> ste-state: {state}", newState);
					if (newState != state.Status)
					{
						state.PreviousStatus = state.Status;
						state.Status = newState;
						if(state.PreviousStatus != state.Status)
						{
							Log.Debug(">>> Notify {oldState} -> {newState}", state.PreviousStatus, state.Status);
						}
					}
					//if (newState > state.Status)
					//{
					//	state.Status = newState;
					//}
				}
			});
		}

		public Task ProcessAsync(Element definition, ElementState result)
		{
			//result.Status = Status.Ok;

			var rules = new Rules.RuleProcessor(definition.StateRules);

			rules.Run(result);

			return Task.CompletedTask;
		}

		public Task ProcessAsync(Element definition, IEnumerable<ElementState> states)
		{
			var rules = new Rules.RuleProcessor(definition.StateRules);

			foreach (var result in states)
			{
				//result.Status = Status.Ok;

				rules.Run(result);
			}

			return Task.CompletedTask;
		}
	}
}


//	/// <summary>
//	/// </summary>
//	/// <remarks>
//	/// @TODO:
//	/// - 'time' based 'consecutive hits'
//	/// - 'value doesn't change for x hits'
//	/// - 'Value > last value for x hits'
//	/// - std deviation stuff
//	/// </remarks>
//	class RuleProcessor : IRuleProcessor
//    {
//		// rule-id -> definition-id -> state-id -> history
//		private readonly Dictionary<long,   // rule-id
//			Dictionary<string,              // definition-id
//				Dictionary<string,          // state-id
//					List<Tuple<DateTime, bool>>>>> _results = new Dictionary<long, Dictionary<string, Dictionary<string, List<Tuple<DateTime, bool>>>>>();

//		private readonly IExpressionEvaluator _expressionEvaluator;

//		public RuleProcessor(IExpressionEvaluator expressionEvaluator)
//        {
//			_expressionEvaluator = expressionEvaluator;
//		}

//		/// <summary>
//		/// Process multiple states as a single action - used for a-type
//		/// </summary>
//		/// <param name="definition"></param>
//		/// <param name="states"></param>
//		/// <returns></returns>
//		public Task ProcessAsync(Element definition, IEnumerable<ElementState> states)
//		{
//			foreach (var state in states)
//			{
//				state.Status = Status.Unknown;

//				foreach (var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null && r.StateModifiers.Any()))
//				{
//					var result = _expressionEvaluator.Evaluate(rule.Expression, state);

//					SaveResult(rule, definition.Id, state.Id, result);

//					state.Status = GetStatus(rule, definition.Id, state.Id, state.Status);
//				}
//			}

//			// Remove any history we don't need any more
//			Cleanup(definition.Id, states);

//			return Task.CompletedTask;
//		}


//		public Task ProcessAsync(Element definition, ElementState state)
//		{
//			// run all valid rules
//			foreach(var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null && r.StateModifiers.Any()))
//			{
//				var result = _expressionEvaluator.Evaluate(rule.Expression, state);

//				SaveResult(rule, definition.Id, state.Id, result);

//				state.Status = GetStatus(rule, definition.Id, state.Id, state.Status);
//			}

//			return Task.CompletedTask;
//		}


//		private Status GetStatus(Rule rule, string definitionId, string stateId, Status currentStatus)
//		{
//			var status = Status.Ok; // default to OK
//			var history = GetRuleHistory(rule.Id, definitionId, stateId).OrderByDescending(h => h.Item1);

//			foreach (var mod in rule.StateModifiers.OrderByDescending(m => m.ConsecutiveHits.HasValue ? m.ConsecutiveHits.Value : 0))
//			{
//				// Hit based
//				if (!mod.ConsecutiveHits.HasValue || history.Count() >= mod.ConsecutiveHits.Value)
//				{
//					var range = history.Take(mod.ConsecutiveHits.HasValue ? Math.Max(mod.ConsecutiveHits.Value, 1) : 1); // Consecutive hits of 'zero' then. What does that mean?

//					// All true. Use this modifier
//					if (range.All(x => x.Item2))
//					{
//						status = mod.Value;
//						break;
//					}
//				}
//			}

//			return status > currentStatus
//				? status
//				: currentStatus;
//		}


//		private void SaveResult(Rule rule, string definitionId, string stateId, bool result)
//		{
//			var ruleHistory = GetRuleHistory(rule.Id, definitionId, stateId);
//			ruleHistory.Add(new Tuple<DateTime, bool>(DateTime.UtcNow, result));

//			// trunc history
//			var maxConsecutiveHist = rule.StateModifiers.Max(m => m.ConsecutiveHits);
//			while (ruleHistory.Count > maxConsecutiveHist)
//			{
//				ruleHistory.RemoveAt(0);
//			}
//		}


//		private List<Tuple<DateTime, bool>> GetRuleHistory(long id, string definitionId, string stateId)
//		{
//			Dictionary<string, Dictionary<string, List<Tuple<DateTime, bool>>>> definitions;

//			if (_results.ContainsKey(id))
//			{
//				definitions = _results[id];
//			}
//			else
//			{
//				definitions = new Dictionary<string, Dictionary<string, List<Tuple<DateTime, bool>>>>();
//				_results.Add(id, definitions);
//			}

//			Dictionary<string, List<Tuple<DateTime, bool>>> states;

//			if (definitions.ContainsKey(definitionId))
//			{
//				states = definitions[definitionId];
//			}
//			else
//			{
//				states = new Dictionary<string, List<Tuple<DateTime, bool>>>();
//				definitions.Add(definitionId, states);
//			}

//			List<Tuple<DateTime, bool>> history;

//			if (states.ContainsKey(stateId))
//			{
//				history = states[stateId];
//			}
//			else
//			{
//				history = new List<Tuple<DateTime, bool>>();
//				states.Add(stateId, history);
//			}

//			return history;
//		}


//		/// <summary>
//		/// Remove any history if there are no instances of 'state' for this definition-id
//		/// </summary>
//		private void Cleanup(string definitionId, IEnumerable<ElementState> states)
//		{
//			var stateIds = states.Select(x => x.Id);

//			// each rule-id
//			foreach(var ruleId in _results.Keys)
//			{
//				// each definition-id
//				foreach(var definition in _results[ruleId].Keys.Where(k => k == definitionId))
//				{
//					var expired = new List<string>();

//					// each state-id
//					foreach (var stateId in _results[ruleId][definition].Keys)
//					{
//						if (!stateIds.Contains(stateId))
//						{
//							expired.Add(stateId);
//						}
//					}

//					foreach(var stateId in expired)
//					{
//						Log.Debug("Expiring: {rule} -> {definition} -> {state}", ruleId, definitionId, stateId);
//						_results[ruleId][definition].Remove(stateId);
//					}
//				}
//			}
//		}
//	}
//}
