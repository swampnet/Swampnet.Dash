﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Serialization;

namespace Swampnet.Rules
{
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
		public List<ActionDefinition> WhenTrue { get; set; }
		public List<ActionDefinition> WhenFalse { get; set; }

		[XmlAttribute]
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
				foreach(var action in GetValidActionDefinitions(context, WhenTrue, true))
				{
					_actionDefinitions[action.ActionName].Execute(action, context);
				}
			}
			else
			{
				foreach (var action in GetValidActionDefinitions(context, WhenFalse, false))
				{
					_actionDefinitions[action.ActionName].Execute(action, context);
				}
			}

			return result;
		}


		public static IExecuteConfiguration ForAction(string actionName)
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


		// @todo: This is working, but it sure is ugly!
		private IEnumerable<ActionDefinition> GetValidActionDefinitions(IContext context, IEnumerable<ActionDefinition> actions, bool result)
		{
			if (_history.ContainsKey(context.Id))
			{
				var history = _history[context.Id].OrderByDescending(h => h.Timestamp);

				if(actions != null)
				{
					foreach (var actionDefinition in actions.OrderByDescending(a => a.ConsecutiveHits.HasValue ? a.ConsecutiveHits.Value : 0))
					{
						if (_actionDefinitions.ContainsKey(actionDefinition.ActionName))
						{
							// We don't care about history, or we have enough history to process it
							if (!actionDefinition.ConsecutiveHits.HasValue || history.Count() >= actionDefinition.ConsecutiveHits.Value)
							{
								// grab exactly enough history to satisfy the consecutive hits value
								var range = history.Take(actionDefinition.ConsecutiveHits.HasValue ? Math.Max(actionDefinition.ConsecutiveHits.Value, 1) : 1); // Consecutive hits of 'zero' en. What does that mean?

								// All true, return this action																										   // All true. Use this modifier
								if (range.All(x => x.Result == result))
								{
									yield return actionDefinition;

									if (actionDefinition.HaltOnExecute)
									{
										break; // Don't process any more...
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
