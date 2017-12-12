using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common;
using Swampnet.Dash;

namespace UnitTests
{
	[TestClass]
	public class Compare_Tests
	{
		[TestMethod]
		public void Compare_DashboardItem_01()
		{
			var lhs = new DashboardItem()
			{
				Id = "01",
				Status = Status.Ok,
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
				Status = Status.Ok,
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-02", "value-02"),
					new Property("property-03", "value-03")
				}
			};

			Assert.IsTrue(Compare.DashboardItems(lhs, rhs));
		}


		[TestMethod]
		public void Compare_DashboardItem_02()
		{
			var lhs = new DashboardItem()
			{
				Id = "01",
				Status = Status.Ok,
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
				Status = Status.Ok,
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-03", "value-03"),
					new Property("property-02", "value-02")
				}
			};

            Assert.IsTrue(Compare.DashboardItems(lhs, rhs));
        }


        // Different property value
        [TestMethod]
		public void Compare_DashboardItem_03()
		{
			var lhs = new DashboardItem()
			{
				Id = "01",
				Status = Status.Ok,
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
				Status = Status.Ok,
				TimestampUtc = DateTime.MinValue,
				Output = new System.Collections.Generic.List<Property>()
				{
					new Property("property-01", "value-01"),
					new Property("property-03", "value-03_xx"),
					new Property("property-02", "value-02")
				}
			};

            Assert.IsFalse(Compare.DashboardItems(lhs, rhs));
        }


        [TestMethod]
        public void Property_Equals_01()
        {
            var lhs = new Property() { Category = "cat", Name = "name", Value = "val" };
            var rhs = new Property() { Category = "cat", Name = "name", Value = "val" };

            Assert.IsTrue(Compare.Properties(lhs, rhs));
        }

        [TestMethod]
        public void Property_Equals_NullCategory()
        {
            var lhs = new Property() { Category = null, Name = "name", Value = "val" };
            var rhs = new Property() { Category = null, Name = "name", Value = "val" };

            Assert.IsTrue(Compare.Properties(lhs, rhs));
        }

        [TestMethod]
        public void Property_Equals_Different_Category()
        {
            var lhs = new Property() { Category = "cat", Name = "name", Value = "val" };
            var rhs = new Property() { Category = "catX", Name = "name", Value = "val" };

            Assert.IsFalse(Compare.Properties(lhs, rhs));
        }

        [TestMethod]
        public void Property_Equals_Different_Name()
        {
            var lhs = new Property() { Category = "cat", Name = "name", Value = "val" };
            var rhs = new Property() { Category = "cat", Name = "nameX", Value = "val" };

            Assert.IsFalse(Compare.Properties(lhs, rhs));
        }

        [TestMethod]
        public void Property_Equals_Different_Value()
        {
            var lhs = new Property() { Category = "cat", Name = "name", Value = "val" };
            var rhs = new Property() { Category = "cat", Name = "name", Value = "valX" };

            Assert.IsFalse(Compare.Properties(lhs, rhs));
        }

        [TestMethod]
        public void Boosh()
        {
            // Not actuall a test, just want a serialised TestDefinition!
            var def = new TestDefinition()
            {
                Id = "test-definition",
                Description = "Some test definition",
                Heartbeat = TimeSpan.FromSeconds(2),
                Type = "SomeClrType",
                Parameters = new System.Collections.Generic.List<Property>()
                {
                    new Property("arg-01", "val-01")
                },
                StateRules = new System.Collections.Generic.List<Rule>()
                {
                    new Rule()
                    {
                        Expression = new Expression()
                        {
                            IsActive = true,
                            Operator = Expression.RuleOperatorType.MATCH_ALL,
                            Children = new []
                            {
                                new Expression()
                                {
                                    IsActive = true,
                                    Operand = Expression.RuleOperandType.Property,
                                    Argument = "value",
                                    Operator = Expression.RuleOperatorType.GT,
                                    Value = "100.0"
                                }
                            }
                        },
                        StateModifiers = new System.Collections.Generic.List<StateModifier>()
                        {
                            new StateModifier()
                            {
                                Order = 0,
                                ConsecutiveHits = 5,
                                Value = Status.Warn
                            },
                            new StateModifier()
                            {
                                Order = 1,
                                ConsecutiveHits = 10,
                                Value = Status.Alert
                            }

                        }
                    }
                }
            };

            var xml = def.ToXmlString();
        }
    }
}
