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
            _items.RemoveAll(x => x.Output.IntValue("stage") == 5);

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
            item.Output.Add(new Property("updated-on", DateTime.Now.ToString("s")));
            item.Output.Add(new Property("id", item.Id));
			item.Output.Add(new Property("stage", "0"));
			//item.Status = "newbie";
            _items.Add(item);
			Log.Debug("Creating new item: {id}", item.Id);
		}

		//private readonly string[] _stages = new[]
		//{
		//	"newbie",
		//	"pending",
		//	"in-progress",
		//	"halfway",
		//	"nearly-finished",
		//	"finished"
		//};

		private void Update(DashboardItem item)
        {
			var stage = item.Output.IntValue("stage");

			if(_rnd.NextDouble() < 0.2)
			{
				stage++;
				Log.Debug("Item {item} -> stage {stage}", item.Id, stage);
				item.Output.Get("stage").Value = stage.ToString();
				item.Output.Get("updated-on").Value = DateTime.UtcNow.ToString("s");
			}

			var timeInGroup = DateTime.UtcNow - item.Output.DateTimeValue("updated-on");

			if(timeInGroup.TotalSeconds < 20)
			{
				item.Status = "";
			}
			else if (timeInGroup.TotalSeconds < 30)
			{
				item.Status = "warn";
			}
			else
			{
				item.Status = "alert";
			}
		}
	}
}
