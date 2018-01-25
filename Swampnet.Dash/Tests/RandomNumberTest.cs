using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swampnet.Dash.Tests
{
	class RandomNumberTest : TestBase
    {
        private readonly Random _rnd = new Random();

        protected override Task UpdateAsync()
        {
            var from = Definition.Parameters.IntValue("min");
            var to = Definition.Parameters.IntValue("max");
            var value = _rnd.Next(from, to);

            State.Output.AddOrUpdate("value", value);

			return Task.CompletedTask;
        }



        public override TestMeta Meta => new TestMeta()
		{
			Type = GetType().Name,
			Description = "Generate a random number between [min] and [max]",
			Parameters = new []
			{
				new Property(Constants.MANDATORY_CATEGORY, "min", "Minimum number"),
				new Property(Constants.MANDATORY_CATEGORY, "max", "Maximum number")
			},
			Output = new[]
			{
				new Property("value", "Random number between [min] and [max]")
			}
		};

	}
}
