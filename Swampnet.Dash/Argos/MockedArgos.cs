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

        public Task<ArgosResult> RunAsync(DashboardItemDefinition argosDefinition)
        {
            var result = new ArgosResult();
            result.ArgosId = argosDefinition.Id;

            // Clean up finished items
            _items.RemoveAll(x => x.Output.IntValue("stage") == 5);

			foreach (var item in _items)
			{
				Update(item);
			}

			// Generate new items
			if (!_items.Any())
            {
                CreateNewbie(argosDefinition);
            }
            else
            {
                if(_rnd.NextDouble() < 0.3)
                {
                    CreateNewbie(argosDefinition);
                }
            }


			// Return a copy not our actual items...
			result.Items = _items.Select(i => i.Copy()).ToList();

			return Task.FromResult(result);
        }


        private void CreateNewbie(DashboardItemDefinition argosDefinition)
        {
            var item = new DashboardItem(argosDefinition.Parameters.StringValue("pre") +_id);
            item.Output.Add(new Property("updated-on", DateTime.Now.ToString("s")));
			item.Output.Add(new Property("time-in-group", 0));
			item.Output.Add(new Property("stage", "0"));
            _items.Add(item);
			_id++;
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
			// Update stage
			var stage = item.Output.IntValue("stage");

			if(stage == 0 || _rnd.NextDouble() < 0.2)
			{
				stage++;
				item.Output.Get("stage").Value = stage.ToString();
				item.Output.Get("updated-on").Value = DateTime.UtcNow.ToString("s");
			}

			// Update status
			var timeInGroup = DateTime.UtcNow - item.Output.DateTimeValue("updated-on");
			item.Output.Get("time-in-group").Value = timeInGroup.TotalSeconds.ToString("0.0");
		}
	}
}
