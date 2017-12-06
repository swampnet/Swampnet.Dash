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
        private DateTime _lastNewbie = DateTime.MinValue;
        private int _id = 0;
		private Random _rnd = new Random();

        public Task<ArgosResult> RunAsync(ArgosDefinition argosDefinition)
        {
            var result = new ArgosResult();
            result.ArgosId = argosDefinition.Id;

            // Clean up finished items
            _items.RemoveAll(x => x.Status == "finished");

            if (_lastNewbie < DateTime.Now.AddSeconds(-20))
            {
				if(_rnd.Next(100) > 50)
				{
					Log.Debug("Creating new item");
					var item = new DashboardItem(++_id);
					item.Output.Add(new Property("CreatedOn", DateTime.Now));
					item.Output.Add(new Property("Id", item.Id));
					item.Output.Add(new Property("age", ""));
					item.Status = "newbie";
					_items.Add(item);
					_lastNewbie = DateTime.Now;
				}
			}


            foreach (var item in _items)
            {
                var age = DateTime.Now - item.Output.DateTimeValue("CreatedOn");
				
				// This *should* trigger an update every heartbeat
				item.Output.Get("age").Value = age.TotalSeconds.ToString();

                if (age.TotalSeconds > 150)
                {
                    item.Status = "finished";
                }
                else if (age.TotalSeconds > 110)
                {
                    item.Status = "nearly-finished";
                }
                else if (age.TotalSeconds > 80)
                {
                    item.Status = "halfway";
                }
                else if (age.TotalSeconds > 45)
                {
                    item.Status = "in-progress";
                }
                else if(age.TotalSeconds > 10)
                {
                    item.Status = "pending";
                }
            }

            result.Items = _items.ToArray();

            return Task.FromResult(result);
        }
    }
}
