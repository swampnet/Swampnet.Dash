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
        private DateTime _lastRunUtc = DateTime.MinValue;

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
			var rs = await RunAsync();

			rs.Id = Definition.Id;
			_lastRunUtc = DateTime.UtcNow;

			State = rs;

			return rs;
		}


		/// <summary>
		/// Set configuration for test
		/// </summary>
		/// <param name="testDefinition"></param>
		public void Configure(Element testDefinition)
		{
			_testDefinition = testDefinition;
		}


		/// <summary>
		/// True if this test is due
		/// </summary>
		public bool IsDue
		{
			get
			{
				return _testDefinition != null && ((DateTime.UtcNow - _lastRunUtc) > _testDefinition.Heartbeat);
			}
		}
		#endregion

		protected abstract Task<ElementState> RunAsync();
    }
}
