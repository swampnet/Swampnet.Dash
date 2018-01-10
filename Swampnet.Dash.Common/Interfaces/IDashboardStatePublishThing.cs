using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IDashboardStatePublishThing
    {
		Task BroadcastState(ITest test);
	}
}
