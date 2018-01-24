using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Rules
{
	public class RuleProcessor
	{
		private readonly IEnumerable<Rule> _rules;

		public RuleProcessor(IEnumerable<Rule> rules)
		{
			_rules = rules;
		}


		public void Run(IContext context)
		{
			// GHrab a copy of the rules
			var rules = new List<Rule>(_rules);
			var count = int.MaxValue;

			while (count > 0 && rules.Any())
			{
				count = 0;
				foreach (var rule in rules.OrderBy(r => r.Order).ToArray())
				{
					if (rule.Run(context))
					{
						// @todo: Execute actions here rather than from within Rule?

						rules.Remove(rule);
						count++;
					}
				}
			}
		}
	}
}
