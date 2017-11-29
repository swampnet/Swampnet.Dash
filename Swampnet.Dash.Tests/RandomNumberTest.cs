using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Tests
{
    class RandomNumberTest : ITest
    {
        private readonly Random _rnd = new Random();

        public DashItem Update(DashItem dashItem)
        {
            if(dashItem == null)
            {
                dashItem = new DashItem();
            }

            var di = dashItem.Properties.SingleOrDefault(d => d.Name == "value");
            if(di == null)
            {
                di = new Property();
                dashItem.Properties.Add(di);
            }

            di.Value = _rnd.NextDouble() * 100.0;

            return dashItem;
        }
    }
}
