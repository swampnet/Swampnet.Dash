using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Swampnet.Dash
{
    public static class StateRuleExtensions
    {
		public static int MaxRuleStateModifierConsecutiveCount_HolyShitChangeThisNameOmg(this IEnumerable<Common.Entities.Rule> source)
		{
			int x = 0;
			if (source != null)
			{
				var xx = source
					.SelectMany(r => r.StateModifiers)
					.Where(r => r.ConsecutiveHits.HasValue);

				if (xx.Any())
				{
					x = xx.Max(m => m.ConsecutiveHits.Value);
				}
			}
			return x;
		}

	}
}
