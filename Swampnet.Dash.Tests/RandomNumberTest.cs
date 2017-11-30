using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Swampnet.Dash.Tests
{
    public class RandomNumberTest : ITest
    {
        private readonly Random _rnd = new Random();

        public Task<TestResult> RunAsync(TestDefinition testDefinition)
        {
            var update = new TestResult(testDefinition.Id);
            var from = testDefinition.Parameters.IntValue("min");
            var to = testDefinition.Parameters.IntValue("max");
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
            
            return Task.FromResult(update);
        }
    }
}
