using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Rules
{
    public interface IContext
    {
		/// <summary>
		/// We need Id for the internal result history
		/// </summary>
		string Id { get; }

		/// <summary>
		/// We need a timestamp for the internal result history
		/// </summary>
		DateTime Timestamp { get; }

		/// <summary>
		/// Rule engine uses this to pull out property values at runtime
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object GetParameterValue(string name);
	}
}
