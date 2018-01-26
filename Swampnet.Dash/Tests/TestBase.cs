using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Tests
{
    abstract class TestBase : ITest
    {
        private Element _testDefinition;

		#region ITest
		public Element Definition => _testDefinition;

		public string Id => _testDefinition.Id;

		public ElementState State { get; private set; }

		abstract public TestMeta Meta { get; }

		/// <summary>
		/// Run test
		/// </summary>
		/// <returns></returns>
		public async Task<ElementState> ExecuteAsync()
		{
			await UpdateAsync();

			State.Timestamp = DateTime.UtcNow;

			return State;
		}


		/// <summary>
		/// Set configuration for test
		/// </summary>
		/// <param name="definition"></param>
		public void Configure(Element definition)
		{
			_testDefinition = definition;

			if(Meta.Parameters != null)
			{
				foreach (var mandatoryParameter in Meta.Parameters.Where(p => string.IsNullOrEmpty(p.Category) || p.Category.EqualsNoCase(Constants.MANDATORY_CATEGORY)))
				{
					if (string.IsNullOrEmpty(Definition.Parameters.StringValue(mandatoryParameter.Name)))
					{
						throw new ArgumentNullException(mandatoryParameter.Name);
					}
				}
			}

			State = new ElementState(definition.Id, definition.Id);
		}


		/// <summary>
		/// True if this test is due
		/// </summary>
		public bool IsDue
		{
			get
			{
				return _testDefinition != null && ((DateTime.UtcNow - State.Timestamp) > _testDefinition.Heartbeat);
			}
		}
		#endregion

		protected abstract Task UpdateAsync();
    }
}
