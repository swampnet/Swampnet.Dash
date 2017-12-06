using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Argos
{
    class MockedArgos : IArgos
    {
        private readonly List<DashboardItem> _items = new List<DashboardItem>();
        private DateTime _lastNewbie = DateTime.MinValue;
        private int _id = 0;

        public Task<ArgosResult> RunAsync(ArgosDefinition argosDefinition)
        {
            var result = new ArgosResult();
            result.ArgosId = argosDefinition.Id;

            // Clean up finished items
            _items.RemoveAll(x => x.Status == "finished");

            // Add a newbie every 30s
            if (_lastNewbie < DateTime.Now.AddSeconds(-30))
            {
                var item = new DashboardItem(++_id);
                item.Output.Add(new Property("CreatedOn", DateTime.Now));
                item.Output.Add(new Property("Id", item.Id));
                item.Status = "newbie";
                _items.Add(item);
                _lastNewbie = DateTime.Now;
            }

            foreach (var item in _items)
            {
                var age = DateTime.Now - item.Output.DateTimeValue("CreatedOn");

                if (age.TotalSeconds > 60)
                {
                    item.Status = "finished";
                }
                else if (age.TotalSeconds > 50)
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

            result.Items = _items.ToArray();

            return Task.FromResult(result);
        }

        //TestMeta Meta
        //{
        //    get;
        //}
    }
}
