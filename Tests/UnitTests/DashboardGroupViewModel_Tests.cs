using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swampnet.Dash.Client.Wpf.ViewModels;

namespace UnitTests
{
	[TestClass]
	public class DashboardGroupViewModel_Tests
	{
		[TestMethod]
		public void TestMethod1()
		{
			var dash = Mock.Dashboard;

			dash.ElementsPerRow = 4;

			var group = new DashboardGroupViewModel(dash, dash.Groups.First());

			// Add some elements
			group.ElementStates.Add(new ElementStateViewModel(dash, Mock.ElementDefinition, "test-01"));

			Assert.AreEqual(1, group.RowCount);

			group.ElementStates.Add(new ElementStateViewModel(dash, Mock.ElementDefinition, "test-01"));
			group.ElementStates.Add(new ElementStateViewModel(dash, Mock.ElementDefinition, "test-01"));
			group.ElementStates.Add(new ElementStateViewModel(dash, Mock.ElementDefinition, "test-01"));

			Assert.AreEqual(1, group.RowCount);

			// 5 items should push us onto row 2
			group.ElementStates.Add(new ElementStateViewModel(dash, Mock.ElementDefinition, "test-01"));

			Assert.AreEqual(2, group.RowCount);
		}
	}
}
