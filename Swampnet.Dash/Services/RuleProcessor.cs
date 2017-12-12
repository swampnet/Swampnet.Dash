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
        private readonly ITestHistory _testHistory;

        public RuleProcessor(ITestHistory testHistory)
        {
            _testHistory = testHistory;
        }


        public Task ProcessTestResultAsync(TestDefinition definition, TestResult result)
        {
            var currentState = _testHistory.GetCurrentState(definition);
            result.Status = currentState == null ? Status.Unknown : currentState.Status;
            var oldStatus = result.Status;

            if(definition.StateRules != null)
            {
                bool resetState = true;

                // Check out any rules with both an expression and state modifiers.
                foreach(var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null))
                {
                    var eval = new ExpressionEvaluator();

                    // @TODO: So, we actually need to run this expression over the last (x) results. (x) being the
                    //        definition.MaxRuleStateModifierConsecutiveCount_HolyShitChangeThisNameOmg plus
                    //        this latest result (unless we add the result to the history earlier in which case we 
                    //        can just pull the history)

                    // Evaluate expression against the current test result
                    if (eval.Evaluate(rule.Expression, result))
                    {
                        // Find which state rule to apply
                        var modifier = GetModifier(definition, rule.StateModifiers, result);

                        // Aggregate: Always take the 'worst' status
                        if (modifier.Value > result.Status)
                        {
                            result.Status = modifier.Value;
                            resetState = false;
                        }
                    }
                }

                // None of the rules fired. Reset to default status
                if (result.Status == Status.Unknown || resetState)
                {
                    result.Status = Status.Ok;
                }
            }
            
            // Check for change of state
            if(oldStatus != result.Status)
            {
                // @todo: Status has changed. Do something!
                Log.Debug("Test {id} status changed {old} -> {new}", 
                    definition.Id, 
                    oldStatus, 
                    result.Status);
            }

            _testHistory.AddTestResult(definition, result);

            return Task.CompletedTask;
        }


        /// <summary>
        /// Figure out which modifier to use
        /// </summary>
        private StateModifier GetModifier(TestDefinition definition, List<StateModifier> modifiers, TestResult result)
        {
            IEnumerable<TestResult> history = _testHistory.GetHistory(definition);

            foreach(var modifier in modifiers.OrderBy(m => m.Order))
            {
                // Ok, shit, we need to run the expression over the last (x) results...
                // This needs a rethink. This should be happening up there ^^ in ProcessTestResultAsync
            }

            return modifiers.OrderBy(m => m.Order).First(); // @HACK: Just grab the first
        }
    }
}
