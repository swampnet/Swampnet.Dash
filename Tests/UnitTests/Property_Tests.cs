using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swampnet.Dash.Common.Entities;

namespace UnitTests
{
	[TestClass]
	public class Property_Tests
	{
		[TestMethod]
		public void Property_Equals_01()
		{
			var lhs = new Property() { Category = "cat", Name = "name", Value = "val" };
			var rhs = new Property() { Category = "cat", Name = "name", Value = "val" };

			var actual = lhs.Equals(rhs);
			var expected = true;

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Property_Equals_NullCategory()
		{
			var lhs = new Property() { Category = null, Name = "name", Value = "val" };
			var rhs = new Property() { Category = null, Name = "name", Value = "val" };

			var actual = lhs.Equals(rhs);
			var expected = true;

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Property_Equals_Different_Category()
		{
			var lhs = new Property() { Category = "cat", Name = "name", Value = "val" };
			var rhs = new Property() { Category = "catX", Name = "name", Value = "val" };

			var actual = lhs.Equals(rhs);
			var expected = false;

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Property_Equals_Different_Name()
		{
			var lhs = new Property() { Category = "cat", Name = "name", Value = "val" };
			var rhs = new Property() { Category = "cat", Name = "nameX", Value = "val" };

			var actual = lhs.Equals(rhs);
			var expected = false;

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Property_Equals_Different_Value()
		{
			var lhs = new Property() { Category = "cat", Name = "name", Value = "val" };
			var rhs = new Property() { Category = "cat", Name = "name", Value = "valX" };

			var actual = lhs.Equals(rhs);
			var expected = false;

			Assert.AreEqual(expected, actual);
		}
	}
}
