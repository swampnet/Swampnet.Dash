using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Swampnet.Dash.Argos
{
    class MockedArgos : IArgos
    {
        private readonly List<DashboardItem> _items = new List<DashboardItem>();
        private int _id = 0;
		private Random _rnd = new Random();

        public Task<ArgosResult> RunAsync(ArgosDefinition argosDefinition)
        {
            var result = new ArgosResult();
            result.ArgosId = argosDefinition.Id;

            // Clean up finished items
            _items.RemoveAll(x => x.Status == "finished");

            // Generate new items
            if (!_items.Any())
            {
                CreateNewbie();
            }
            else
            {
                if(_rnd.NextDouble() < 0.3)
                {
                    CreateNewbie();
                }
            }

            foreach(var item in _items)
            {
                Update(item);
            }

            // Nggg, return a copy not our actual items...
            result.Items = _items.Select(i => new DashboardItem() {
                Id = i.Id,
                Status = i.Status,
                TimestampUtc = i.TimestampUtc,
                Output = i.Output.Select(o => new Property()
                {
                    Category = o.Category,
                    Name = o.Name,
                    Value = o.Value
                }).ToList()
            }).ToArray();

			return Task.FromResult(result);
        }


        private void CreateNewbie()
        {
            var item = new DashboardItem(++_id);
            item.Output.Add(new Property("created-on", DateTime.Now));
            item.Output.Add(new Property("id", item.Id));
            item.Output.Add(new Property("age", ""));
            item.Status = "newbie";
            _items.Add(item);
			Log.Debug("Creating new item: {id}", item.Id);
		}


		private void Update(DashboardItem item)
        {
            var age = DateTime.Now - item.Output.DateTimeValue("created-on");

            //var ageProperty = item.Output.Get("age");
            //ageProperty.Value = age.TotalSeconds.ToString("0.0");

            if (age.TotalSeconds > 50)
            {
                item.Status = "finished";
            }
            else if (age.TotalSeconds > 40)
            {
                item.Status = "nearly-finished";
            }
            else if (age.TotalSeconds > 30)
            {
                item.Status = "halfway";
            }
            else if (age.TotalSeconds > 20)
            {
                item.Status = "in-progress";
            }
            else if (age.TotalSeconds > 10)
            {
                item.Status = "pending";
            }
        }
    }
}
