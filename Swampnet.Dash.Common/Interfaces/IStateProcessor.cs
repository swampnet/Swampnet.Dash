using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
	/// <summary>
	/// Post processing for a State
	/// </summary>
	public interface IStateProcessor
	{
		Task ProcessAsync(Element definition, ElementState state);
		Task ProcessAsync(Element definition, IEnumerable<ElementState> states);
	}
}
