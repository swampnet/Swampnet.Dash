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
		public async Task Add(IEnumerable<ElementState> elements)
		{
			using (var context = new HistoryContext())
			{
				foreach (var result in elements.GroupBy(t => t.Id))
				{
					var root = await context.Roots.SingleOrDefaultAsync(r => r.Name == result.Key);

					if (root == null)
					{
						root = new HistoryRoot()
						{
							Name = result.Key
						};

						context.Roots.Add(root);
					}

					foreach (var t in result)
					{
						foreach (var r in t.Output)
						{
							root.History.Add(new History()
							{
								Name = r.Name,
								Value = r.Value,
								TimestampUtc = t.TimestampUtc
							});
						}
					}
				}

				await context.SaveChangesAsync();
			}
		}



		public async Task<IEnumerable<Variant>> GetHistory(string itemDefinitionId, string propertyName, TimeSpan history)
		{
			var dt = DateTime.UtcNow.Subtract(history);

			using (var context = new HistoryContext())
			{
				context.Configuration.AutoDetectChangesEnabled = false;

				var results = await context.Roots
					.Where(r => r.Name == itemDefinitionId)
					.SelectMany(t => t.History
						.Where(h => h.Name == propertyName && h.TimestampUtc > dt)
						.Select(h => new {h.Id, h.TimestampUtc, h.Value }))
					.OrderByDescending(h => h.TimestampUtc)
					.ToListAsync();

				return results.Select(h => new Variant(h.TimestampUtc, Convert.ToDouble(h.Value)));
			}
		}
	}
}
