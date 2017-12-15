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
    /// 
    /// - Notes
    ///     - Singleton atm - Don't know if I need to keep track of state here or if some other service will be handling that for me...
    ///     - What happens when we have multiple rules?
    /// </summary>
    class RuleProcessor : IRuleProcessor
    {
        private readonly IDashboardItemHistory _testHistory;

        public RuleProcessor(IDashboardItemHistory testHistory)
        {
            _testHistory = testHistory;
        }

		/// <summary>
		/// @TODO:
		/// 
		/// - Only works on test results. Which makes it useless for argos types.
		/// - 'value doesn't change for x hits'
		/// - 'Value > last value for x hits'
		/// - 'average over last x hits > value for y hits'
		/// - std deviation stuff
		/// </summary>
		/// <param name="definition"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Task ProcessTestResultAsync(DashboardItemDefinition definition, DashboardItem result)
		{
			// Save result
			_testHistory.AddTestResult(definition, result);

			if (definition.StateRules != null && definition.StateRules.Any())
			{
				var history = _testHistory.GetHistory(definition);

				foreach (var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null && r.StateModifiers.Any()))
				{
					var eval = new ExpressionEvaluator();

					// Run expression against all results. Pretty sure we should just be storing this somewhere rather than re-calculate it each time
					var results = new List<Tuple<DateTime, bool>>();
					foreach(var r in history)
					{
						results.Add(new Tuple<DateTime, bool>(
							r.TimestampUtc,
							eval.Evaluate(rule.Expression, r)));
					}

					// Find StateModifier that matches the history
					// tend towards the modifier with the highest ConsecutiveHits (So a modifier that requires 10 consecutive hits beats one that
					// only requires 1
					foreach (var mod in rule.StateModifiers.OrderByDescending(m => m.ConsecutiveHits.HasValue ? m.ConsecutiveHits.Value : 0))
					{
						// Hit based
						if (!mod.ConsecutiveHits.HasValue || results.Count >= mod.ConsecutiveHits.Value)
						{
							var range = results.Take(mod.ConsecutiveHits.HasValue ? Math.Max(mod.ConsecutiveHits.Value, 1) : 1); // Consecutive hits of 'zero' then. What does that mean?
							if (range.All(x => x.Item2))
							{
								// All true. Use this modifier
								//Log.Debug("{hits} Consecutive hits - status: {statue}", mod.ConsecutiveHits.Value, mod.Value);
								result.Status = mod.Value;
								break;
							}
						}
					}

					// @todo: Time based
				}
			}

			// Default status if not set
			if(result.Status == Status.Unknown)
			{
				result.Status = Status.Ok;
			}

			return Task.CompletedTask;
		}
    }
}
