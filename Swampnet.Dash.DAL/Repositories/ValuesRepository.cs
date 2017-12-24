using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Swampnet.Dash.DAL.Repositories
{
	class ValuesRepository : IValuesRepository
	{
		public async Task<IEnumerable<Varient>> GetHistory(string itemDefinitionId, string propertyName, TimeSpan history)
		{
			var dt = DateTime.UtcNow.Subtract(history);

			using (var context = new HistoryContext())
			{
				context.Configuration.AutoDetectChangesEnabled = false;

				var results = await context.Roots
					.Where(r => r.Name == itemDefinitionId)
					.SelectMany(t => t.History
						.Where(h => h.Name == propertyName && h.TimestampUtc > dt)
						.Select(h => new { h.TimestampUtc, h.Value }))
					.OrderByDescending(h => h.TimestampUtc)
					.ToListAsync();

				return results.Select(x => new Varient(x.TimestampUtc, Convert.ToDouble(x.Value)));
			}
		}
	}
}
