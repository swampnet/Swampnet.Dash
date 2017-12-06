using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swampnet.Dash.Common.Entities;

namespace UnitTests
{
	[TestClass]
	public class DashboardItem_Tests
	{
		[TestMethod]
		public void DashboardItem_Equals_01()
		{
			var lhs = new DashboardItem()
			{
				Id = "01",
				Status = "test",
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-02", "value-02"),
					new Property("property-03", "value-03")
				}
			};

			var rhs = new DashboardItem()
			{
				Id = "01",
				Status = "test",
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-02", "value-02"),
					new Property("property-03", "value-03")
				}
			};

			Assert.IsTrue(lhs.Equals(rhs));
		}


		[TestMethod]
		public void DashboardItem_Equals_02()
		{
			var lhs = new DashboardItem()
			{
				Id = "01",
				Status = "test",
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-02", "value-02"),
					new Property("property-03", "value-03")
				}
			};

			// Properties are the same, but in a different order
			var rhs = new DashboardItem()
			{
				Id = "01",
				Status = "test",
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-03", "value-03"),
					new Property("property-02", "value-02")
				}
			};

			Assert.IsTrue(lhs.Equals(rhs));
		}


		// Different property value
		[TestMethod]
		public void DashboardItem_Equals_03()
		{
			var lhs = new DashboardItem()
			{
				Id = "01",
				Status = "test",
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-02", "value-02"),
					new Property("property-03", "value-03")
				}
			};

			var rhs = new DashboardItem()
			{
				Id = "01",
				Status = "test",
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-03", "value-03_xx"),
					new Property("property-02", "value-02")
				}
			};

			Assert.IsFalse(lhs.Equals(rhs));
		}

	}
}
