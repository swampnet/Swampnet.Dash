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
	/// This migh all belong in DashRepo...
	/// </summary>
	class State : IState
	{
		// [dashId => [dashItem.Id => dashItem]]
		private readonly Dictionary<string, Dictionary<string, DashboardItem>> _state = new Dictionary<string, Dictionary<string, DashboardItem>>();

		public Task<IEnumerable<DashboardItem>> GetDashItemsAsync(string dashId)
		{
			IEnumerable<DashboardItem> dashItems = null;

			if (_state.ContainsKey(dashId))
			{
				dashItems = _state[dashId].Values;
			}

			return Task.FromResult(dashItems);
		}


		public Task SaveDashItemsAsync(string dashId, IEnumerable<DashboardItem> dashItems)
		{
			Dictionary<string, DashboardItem> x;

			lock (_state)
			{
				if (_state.ContainsKey(dashId))
				{
					x = _state[dashId];
				}
				else
				{
					x = new Dictionary<string, DashboardItem>();
					_state.Add(dashId, x);
				}
			}

			Merge(x, dashItems);

			return Task.CompletedTask;
		}


		private void Merge(Dictionary<string, DashboardItem> x, IEnumerable<DashboardItem> dashItems)
		{
			lock (x)
			{
				foreach (var di in dashItems)
				{
					if (x.ContainsKey(di.Id))
					{
						x[di.Id] = di;
					}
					else
					{
						x.Add(di.Id, di);
					}
				}
			}
		}
	}
}
