using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Rules
{
	public class Rules
	{
		private readonly IEnumerable<Rule> _rules;

		public Rules(IEnumerable<Rule> rules)
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


	public class Rule
	{
		private class History
		{
			public DateTime Timestamp { get; set; }
			public bool Result { get; set; }
		}

		private readonly static Dictionary<string, RuleConfiguration> _actionDefinitions = new Dictionary<string, RuleConfiguration>();

		// context-id => history
		private readonly static Dictionary<string, List<History>> _history = new Dictionary<string, List<History>>();

		public string Description { get; set; }

		/// <summary>
		/// Rule is true if the expression is true!
		/// </summary>
		public Expression Expression { get; set; }

		/// <summary>
		/// List of actions to perform if expression is true
		/// </summary>
		public List<ActionDefinition> Actions { get; set; }
		public int Order { get; set; }

		/// <summary>
		/// Return true if we executed any actions
		/// </summary>
		public bool Run(IContext context)
		{
			if (Expression == null)
			{
				throw new ArgumentNullException("Expression cannot be null");
			}

			bool result = Expression.Evaluate(context);

			// Store result in history
			Save(context, result);

			// Run any actions that qualify
			if (result)
			{
				if (Actions != null)
				{
					foreach (var action in Actions)
					{
						if (_actionDefinitions.ContainsKey(action.ActionName))
						{
							// @T|ODO: Sort out consecutive hits stuff here?

							var a = _actionDefinitions[action.ActionName];
							if (a != null)
							{
								a.Execute(action, context);
							}
						}
					}
				}
			}

			return result;
		}


		public static IRunConfiguration For(string actionName)
		{
			var cfg = new RuleConfiguration();

			_actionDefinitions[actionName] = cfg;

			return cfg;
		}

		private void Save(IContext context, bool result)
		{
			List<History> history;

			// @todo: Store in history
			if (_history.ContainsKey(context.Id))
			{
				history = _history[context.Id];
			}
			else
			{
				history = new List<History>();
				_history.Add(context.Id, history);
			}

			history.Add(new History()
			{
				Timestamp = context.Timestamp,
				Result = result
			});

			// @todo: cleanup history. Interesting: How do we figure that out?
			//  Possibly look at the rule action settings and figure out how much we need.
			//  - problem with that is we'll get a leak if the id's are transient (like our a-type items)
			//  - This is a problem. How on earth do we know when we're 'done' with a specific id?
			//      - For non-transient stuff (ie, a test that will run for the lifetime of the application) it's easy: We never need to remove it, just
			//        truncate the history based on 'consecutive-hits' or whatever each time we save history.
			//      - For transient stuff it's tricky: At some point we'll just stop getting updates (eg, for a-types an item will eventually complete, at
			//        which point we can clean up the entry for that id - Problem here is that we never get that signal. We just stop receiving updates.
			//          - Can we define some kind of ttl? If we don't get an update for (x) seconds then clear it out? Feels hacky, and where do we define that?
			//          - Also, we'd have to check whenever we added *anything* which could get expensive.
			//              - We could have a background monitor thread I suppose...
		}
	}

	public interface IRunConfiguration
	{
		void Run(Action<ActionDefinition, IContext> action);
	}

	internal class RuleConfiguration :
		IRunConfiguration
	{
		private Action<ActionDefinition, IContext> _action;

		public void Run(Action<ActionDefinition, IContext> action)
		{
			_action = action;
		}

		internal void Execute(ActionDefinition def, IContext context)
		{
			_action.Invoke(def, context);
		}
	}
}
