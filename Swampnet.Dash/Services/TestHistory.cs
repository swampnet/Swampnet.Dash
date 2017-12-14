using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;

namespace Swampnet.Dash.Services
{
    class TestHistory : ITestHistory
    {
        private Dictionary<string, List<TestResult>> _history = new Dictionary<string, List<TestResult>>();


        public void AddTestResult(TestDefinition definition, TestResult result)
        {
            List<TestResult> results;
            if (!_history.ContainsKey(definition.Id))
            {
                results = new List<TestResult>();
                _history.Add(definition.Id, results);
            }
            else
            {
                results = _history[definition.Id];
            }

            // Add a copy
            results.Add(new TestResult()
            {
                TimestampUtc = result.TimestampUtc,
                Status = result.Status,
                TestId = result.TestId,
                Output = result.Output.Select(o => new Property(o.Name, o.Value) { Category = o.Category }).ToList()
            });

            Trunc(definition, results);
        }


        public TestResult GetCurrentState(TestDefinition definition)
        {
            return GetHistory(definition).FirstOrDefault();
        }


        public IEnumerable<TestResult> GetHistory(TestDefinition definition)
        {
            IEnumerable<TestResult> result = null;

            if (_history.ContainsKey(definition.Id))
            {
                result = _history[definition.Id].OrderByDescending(r => r.TimestampUtc);
            }

            return result == null
                ? Enumerable.Empty<TestResult>()
                : result;
        }


        private static void Trunc(TestDefinition definition, List<TestResult> results)
        {
            int max = definition.StateRules.MaxRuleStateModifierConsecutiveCount_HolyShitChangeThisNameOmg() + 1; // Always leave one
            while (results.Count > max) // @TODO: from testDefinition
            {
                results.RemoveAt(0);
            }
        }
    }
}
