using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swampnet.Dash.Client.Wpf.ViewModels;
using Swampnet.Dash.Common.Entities;
using System.Collections.Generic;

namespace UnitTests
{
	[TestClass]
	public class DashItemViewModel_Tests
	{
		[TestMethod]
		public void TestMethod1()
		{
			var vm = new DashboardItemViewModel("test-id", new[]
			{
				new Meta()
				{
					Name = "test",
					Region = "main",
					Type = "static"
				}
			});

			var expected = "test";
			var actual = vm.Main;

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void TestMethod2()
		{
			var vm = new DashboardItemViewModel("test-id", new[]
			{
				new Meta()
				{
					Name = "status",
					Region = "group"
				}
			});

			vm.Update(new DashboardItem() {
				Status = Status.Warn
			});

			var expected = "test-status";
			var actual = vm.Group;

			Assert.AreEqual(expected, actual);
		}

	}
}
