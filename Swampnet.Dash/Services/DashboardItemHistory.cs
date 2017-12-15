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
        private Dictionary<string, List<DashboardItem>> _history = new Dictionary<string, List<DashboardItem>>();


        public void AddTestResult(DashboardItemDefinition definition, DashboardItem result)
        {
            List<DashboardItem> results;
            if (!_history.ContainsKey(definition.Id))
            {
                results = new List<DashboardItem>();
                _history.Add(definition.Id, results);
            }
            else
            {
                results = _history[definition.Id];
            }

            // Add a copy
            results.Add(new DashboardItem()
            {
                TimestampUtc = result.TimestampUtc,
                Status = result.Status,
                Id = result.Id,
                Output = result.Output.Select(o => new Property(o.Name, o.Value) { Category = o.Category }).ToList()
            });

            Trunc(definition, results);
        }


        public DashboardItem GetCurrentState(DashboardItemDefinition definition)
        {
            return GetHistory(definition).FirstOrDefault();
        }


        public IEnumerable<DashboardItem> GetHistory(DashboardItemDefinition definition)
        {
            IEnumerable<DashboardItem> result = null;

            if (_history.ContainsKey(definition.Id))
            {
                result = _history[definition.Id].OrderByDescending(r => r.TimestampUtc);
            }

            return result == null
                ? Enumerable.Empty<DashboardItem>()
                : result;
        }


        private static void Trunc(DashboardItemDefinition definition, List<DashboardItem> results)
        {
            int max = definition.StateRules.MaxRuleStateModifierConsecutiveCount_HolyShitChangeThisNameOmg() + 1; // Always leave one
            while (results.Count > max) // @TODO: from testDefinition
            {
                results.RemoveAt(0);
            }
        }
    }
}
