using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Rules
{
	public interface IExecuteConfiguration
	{
		void Execute(Action<ActionDefinition, IContext> action);
	}

	/// <summary>
	/// Base for our fluent configuration.
	/// </summary>
	internal class RuleConfiguration :
		IExecuteConfiguration
	{
		private Action<ActionDefinition, IContext> _action;

		public void Execute(Action<ActionDefinition, IContext> action)
		{
			_action = action;
		}

		internal void Execute(ActionDefinition def, IContext context)
		{
			_action.Invoke(def, context);
		}
	}
}
