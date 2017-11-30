using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Tests
{
    public class RandomNumberTest : ITest
    {
        private readonly Random _rnd = new Random();

        public TestUpdate Update(TestDefinition testDefinition)
        {
            var update = new TestUpdate(testDefinition.Id);
            var from = Convert.ToInt32(testDefinition.Parameters.Single(p => p.Name == "min").Value);
            var to = Convert.ToInt32(testDefinition.Parameters.Single(p => p.Name == "max").Value);
            var value = _rnd.Next(from, to);

            // HACK
            if(value > 5)
            {
                update.State = "alert";
            }
            else if(value > 4)
            {
                update.State = "warn";
            }
            else
            {
                update.State = "ok";
            }

            update.Properties.Add(new Property("value", value));
            
            return update;
        }
    }
}
