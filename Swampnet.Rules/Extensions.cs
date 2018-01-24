using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Rules
{
    public static class Extensions
    {
		public static ActionDefinitionParameter Definition(this IEnumerable<ActionDefinitionParameter> source, string name)
		{
			return source == null
				? null
				: source.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public static string Value(this IEnumerable<ActionDefinitionParameter> source, string name, string defaultValue = "")
		{
			var def = source.Definition(name);

			return def == null
				? defaultValue
				: def.Value;
		}
	}
}
