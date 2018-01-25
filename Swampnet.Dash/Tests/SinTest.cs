using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;

namespace Swampnet.Dash.Tests
{
	class SinTest : TestBase
	{
		private double _value = 0.0;


		protected override Task UpdateAsync()
		{
			// amplitude * Math.Sin((2 * Math.PI * n * frequency) / sampleRate)

			_value += Definition.Parameters.DoubleValue("step");

			if(_value > 100)
			{
				_value = -_value;
			}

			State.Output.AddOrUpdate("angle", _value);
			State.Output.AddOrUpdate("value", Math.Sin(_value));

			return Task.CompletedTask;
		}

		public override TestMeta Meta => new TestMeta()
		{
			Type = GetType().Name,
			Description = "output a sinewave",
			Output = new[]
			{
				new Property("value", "value")
			}
		};
	}
}
