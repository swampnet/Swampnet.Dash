using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IDashboardStatePublishThing
    {
		Task BroadcastState(ITest test);
		Task BroadcastState(IEnumerable<ArgosResult> items);
	}
}
