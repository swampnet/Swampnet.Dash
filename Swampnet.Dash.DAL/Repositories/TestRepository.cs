using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common;

namespace Swampnet.Dash.DAL
{
    class TestRepository : ITestRepository
    {
        public IEnumerable<ElementDefinition> GetDefinitions()
        {
            return Mock.Tests;
        }



		public void Add(IEnumerable<Element> testResults)
		{
			using (var context = new HistoryContext())
			{
				foreach(var result in testResults.GroupBy(t => t.Id))
				{
					var root = context.Roots.SingleOrDefault(r => r.Name == result.Key);
					if (root == null)
					{
						root = new HistoryRoot()
						{
							Name = result.Key
						};

						context.Roots.Add(root);
					}

					foreach(var t in result)
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

				context.SaveChanges();
			}
		}


		public IEnumerable<Varient> Get(string testId, string property, TimeSpan? history)
		{
			var dt = history.HasValue
				? DateTime.UtcNow.Subtract(history.Value)
				: DateTime.UtcNow.AddSeconds(-60);

			using(var context = new HistoryContext())
			{
				context.Configuration.AutoDetectChangesEnabled = false;
				return context.Roots
					.Where(r => r.Name == testId)
					.SelectMany(t => t.History
						.Where(h => h.Name == property && h.TimestampUtc > dt)
						.Select(h => new { h.TimestampUtc, h.Value }))
					.OrderByDescending(h => h.TimestampUtc)
					.ToList()
					.Select(x => new Varient(x.TimestampUtc, Convert.ToDouble(x.Value)));
			}
		}
	}
}
