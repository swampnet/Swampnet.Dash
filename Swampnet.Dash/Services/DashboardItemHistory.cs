using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;

namespace Swampnet.Dash.Services
{
	internal class DashboardItemHistory : IDashboardItemHistory
    {
        private Dictionary<string, List<ElementState>> _history = new Dictionary<string, List<ElementState>>();


        public void AddTestResult(Element definition, ElementState result)
        {
            List<ElementState> results;
            if (!_history.ContainsKey(definition.Id))
            {
                results = new List<ElementState>();
                _history.Add(definition.Id, results);
            }
            else
            {
                results = _history[definition.Id];
            }

            // Add a copy
            results.Add(new ElementState()
            {
                TimestampUtc = result.TimestampUtc,
                Status = result.Status,
                Id = result.Id,
                Output = result.Output.Select(o => new Property(o.Name, o.Value) { Category = o.Category }).ToList()
            });

            Trunc(definition, results);
        }


        public ElementState GetCurrentState(Element definition)
        {
            return GetHistory(definition).FirstOrDefault();
        }


        public IEnumerable<ElementState> GetHistory(Element definition)
        {
            IEnumerable<ElementState> result = null;

            if (_history.ContainsKey(definition.Id))
            {
                result = _history[definition.Id].OrderByDescending(r => r.TimestampUtc);
            }

            return result == null
                ? Enumerable.Empty<ElementState>()
                : result;
        }


        private static void Trunc(Element definition, List<ElementState> results)
        {
            int max = definition.StateRules.MaxRuleStateModifierConsecutiveCount_HolyShitChangeThisNameOmg() + 1; // Always leave one
            while (results.Count > max) // @TODO: from testDefinition
            {
                results.RemoveAt(0);
            }
        }
    }
}
