using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash;

namespace UnitTests
{
	[TestClass]
	public class Property_Tests
	{
		[TestMethod]
		public void TimeSpanValue_Test_01()
		{
			var properties = new[] {
				new Property("tst-timespan", "PT5S") // 5 seconds
			};

			var ts = properties.TimeSpanValue("tst-timespan");
			Assert.AreEqual(TimeSpan.FromSeconds(5), ts);
		}
	}
}
