using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;

namespace UnitTests
{
	[TestClass]
	public class RuleProcessor_Tests
	{
		[TestMethod]
		public void TestMethod1()
		{
			var definition = new TestDefinition()
			{
				Id = "test",				
			};
			var testHistory = Substitute.For<ITestHistory>();
			testHistory.GetHistory(definition).Returns(x => new[] 
			{
				new Swampnet.Dash.Common.Entities.TestResult()
				{
					Output = new System.Collections.Generic.List<Property>()
					{
						new Property("value", 1)
					}
				}
			});
		}
	}
}
