using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IArgos
    {
		string Id { get; }
		Task<ArgosResult> RunAsync();
		void Configure(Element definition);
		bool IsDue { get; }
		IEnumerable<ElementState> State { get; }
		Element Definition { get; }
		//TestMeta Meta { get; }
	}
}
