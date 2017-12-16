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
        private Dictionary<string, List<Element>> _history = new Dictionary<string, List<Element>>();


        public void AddTestResult(ElementDefinition definition, Element result)
        {
            List<Element> results;
            if (!_history.ContainsKey(definition.Id))
            {
                results = new List<Element>();
                _history.Add(definition.Id, results);
            }
            else
            {
                results = _history[definition.Id];
            }

            // Add a copy
            results.Add(new Element()
            {
                TimestampUtc = result.TimestampUtc,
                Status = result.Status,
                Id = result.Id,
                Output = result.Output.Select(o => new Property(o.Name, o.Value) { Category = o.Category }).ToList()
            });

            Trunc(definition, results);
        }


        public Element GetCurrentState(ElementDefinition definition)
        {
            return GetHistory(definition).FirstOrDefault();
        }


        public IEnumerable<Element> GetHistory(ElementDefinition definition)
        {
            IEnumerable<Element> result = null;

            if (_history.ContainsKey(definition.Id))
            {
                result = _history[definition.Id].OrderByDescending(r => r.TimestampUtc);
            }

            return result == null
                ? Enumerable.Empty<Element>()
                : result;
        }


        private static void Trunc(ElementDefinition definition, List<Element> results)
        {
            int max = definition.StateRules.MaxRuleStateModifierConsecutiveCount_HolyShitChangeThisNameOmg() + 1; // Always leave one
            while (results.Count > max) // @TODO: from testDefinition
            {
                results.RemoveAt(0);
            }
        }
    }
}
