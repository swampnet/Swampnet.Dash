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
        private readonly List<ElementState> _items = new List<ElementState>();
        private int _id = 0;
		private Random _rnd = new Random();
		private Element _definition;
		private DateTime _lastRunUtc = DateTime.MinValue;

		public string Id => _definition == null ? "" : _definition.Id;
		public bool IsDue => _definition != null && ((DateTime.UtcNow - _lastRunUtc) > _definition.Heartbeat);
		public IEnumerable<ElementState> State => _items;
		public Element Definition => _definition;

		public void Configure(Element definition)
		{
			_definition = definition;
		}

		public Task<ArgosResult> RunAsync()
        {
			try
			{
				// Clean up finished items
				_items.RemoveAll(x => x.Output.IntValue("stage") == 5);

				// Update all items
				foreach (var item in State)
				{
					Update(item);
				}

				// Generate new itemsjj
				if (!_items.Any()/* || _rnd.NextDouble() < 0.3*/)
				{
					CreateNewbie(_definition);
				}
			}
			catch (Exception ex)
			{

			}
			finally
			{
				_lastRunUtc = DateTime.UtcNow;
			}

            var result = new ArgosResult();
			result.ArgosId = Id;

			// Return a copy not our actual items...
			//result.Items = State.Select(i => i.Copy()).ToList();
			result.Items = State.ToList();

			return Task.FromResult(result);
        }


        private void CreateNewbie(Element argosDefinition)
        {
            var item = new ElementState(argosDefinition.Id, argosDefinition.Parameters.StringValue("pre") +_id);
            item.Output.Add(new Property("private", "updated-on", DateTime.Now.ToString("s")));
			item.Output.Add(new Property("private", "time-in-group", 0));
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

		private void Update(ElementState item)
		{
			// Update stage
			//Move(item);

			var timeInGroup = DateTime.UtcNow - item.Output.DateTimeValue("updated-on");
			item.Output.AddOrUpdate("time-in-group", timeInGroup.TotalSeconds.ToString("0.0"));
		}


		private void Move(ElementState item)
		{
			var stage = item.Output.IntValue("stage");

			if (stage == 0 || _rnd.NextDouble() < 0.2)
			{
				stage++;
				item.Output.Get("stage").Value = stage.ToString();
				item.Output.Get("updated-on").Value = DateTime.UtcNow.ToString("s");
			}
		}
	}
}
