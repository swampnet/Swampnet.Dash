using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swampnet.Dash.Tests
{
	class RandomNumberTest : TestBase
    {
        private readonly Random _rnd = new Random();

        protected override Task<ElementState> RunAsync()
        {
            var rs = new ElementState();
            var from = Definition.Parameters.IntValue("min");
            var to = Definition.Parameters.IntValue("max");
            var value = _rnd.Next(from, to);

            rs.Output.Add(new Property("value", value));

            return Task.FromResult(rs);
        }



        public override TestMeta Meta
		{
			get
			{
				return new TestMeta()
				{
					Type = GetType().Name,
					Description = "Generate a random number between [min] and [max]",
					Parameters = new []
					{
						new Property("min", "Minimum number"),
						new Property("max", "Maximum number")
					},
					Output = new[]
					{
						new Property("value", "Random number between [min] and [max]")
					}
				};
			}
		}

	}
}
