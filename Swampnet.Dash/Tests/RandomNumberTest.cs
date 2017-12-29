﻿using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swampnet.Dash.Tests
{
	public class RandomNumberTest : ITest
    {
        private readonly Random _rnd = new Random();

        public Task<ElementState> RunAsync(Element testDefinition)
        {
            var rs = new ElementState();
            var from = testDefinition.Parameters.IntValue("min");
            var to = testDefinition.Parameters.IntValue("max");
            var value = _rnd.Next(from, to);

            rs.Output.Add(new Property("value", value));
            
            return Task.FromResult(rs);
        }


		public TestMeta Meta
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
