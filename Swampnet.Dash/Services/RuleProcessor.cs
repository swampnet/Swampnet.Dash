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
        public Task ProcessTestResultAsync(TestDefinition definition, TestResult result)
        {
            var oldStatus = result.Status;

            if(definition.StateRules != null)
            {
                bool hasRuleInPlay = false;

                // Check out any rules with both an expression and state modifiers.
                foreach(var rule in definition.StateRules.Where(r => r.Expression != null && r.StateModifiers != null))
                {
                    var eval = new ExpressionEvaluator();

                    // Evaluate expression against the current test result
                    if(eval.Evaluate(rule.Expression, result))
                    {
                        hasRuleInPlay = true;

                        // Find which state rule to apply
                        var modifier = rule.StateModifiers.OrderBy(m => m.Order).First(); // @HACK: Just grab the first

                        result.Status = modifier.Value;
                    }
                }

                if (!hasRuleInPlay)
                {
                    result.Status = definition.DefaultStatus;
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

            return Task.CompletedTask;
        }
    }
}
