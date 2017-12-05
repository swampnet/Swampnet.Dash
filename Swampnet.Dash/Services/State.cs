using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;

namespace Swampnet.Dash.Services
{
	/// <summary>
	/// This might all belong in DashRepo...
	/// </summary>
	class State : IState
	{
		private readonly Dictionary<string, DashboardItem> _state = new Dictionary<string, DashboardItem>();

		public Task<IEnumerable<DashboardItem>> GetDashItemsAsync(IEnumerable<string> ids)
		{
			var items = _state.Where(x => ids.Contains(x.Key)).Select(x => x.Value);

			return Task.FromResult(items);
		}

		public Task SaveDashItemsAsync(IEnumerable<DashboardItem> dashItems)
		{
			foreach(var di in dashItems)
			{
				lock (_state)
				{
					if (_state.ContainsKey(di.Id))
					{
						_state[di.Id] = di;
					}
					else
					{
						_state.Add(di.Id, di);
					}
				}
			}

			return Task.CompletedTask;
		}
	}
}
