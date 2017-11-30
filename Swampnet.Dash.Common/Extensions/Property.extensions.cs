using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash
{
    public static class PropertyExtensions
    {
		public static IEnumerable<Property> Values(this IEnumerable<Property> properties, string category, string name)
		{
			return properties.Where(p => p.Category.EqualsNoCase(category) && p.Name.EqualsNoCase(name));
		}


		public static IEnumerable<Property> Values(this IEnumerable<Property> properties, string name)
		{
			return properties.Where(p => p.Name.EqualsNoCase(name));
		}


		/// <summary>
		/// Return property values of all those properties of name 'name'
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static IEnumerable<string> StringValues(this IEnumerable<Property> properties, string name)
		{
			if (properties == null)
			{
				return Enumerable.Empty<string>();

			}

			return properties?.Where(p => p.Name.EqualsNoCase(name)).Select(p => p.Value.ToString());
		}


		/// <summary>
		/// Return the value of a property as a string value
		/// </summary>
		/// <remarks>
		/// Performs a case-insensitive search and returns the value of the specified property as a string.
		/// </remarks>
		/// <param name="properties"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string StringValue(this IEnumerable<Property> properties, string name, string defaultValue = "")
		{
			string v = defaultValue;
			if (properties != null && properties.Any())
			{
				var p = properties.SingleOrDefault(x => x.Name.EqualsNoCase(name));
				if (p != null && p.Value != null)
				{
					v = p.Value.ToString();
				}
			}

			return v;
		}


		/// <summary>
		/// Return the value of a property as an integer value
		/// </summary>
		/// <remarks>
		/// Performs a case-insensitive search and returns the value of the specified property as an integer.
		/// </remarks>
		/// <param name="properties"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static int IntValue(this IEnumerable<Property> properties, string name, int defaultValue = 0)
		{
			int v = defaultValue;

			int.TryParse(properties.StringValue(name, defaultValue.ToString()), out v);

			return v;
		}


		/// <summary>
		/// Return the value of a property as a double value
		/// </summary>
		/// <remarks>
		/// Performs a case-insensitive search and returns the value of the specified property as a double.
		/// </remarks>
		/// <param name="properties"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static double DoubleValue(this IEnumerable<Property> properties, string name, double defaultValue = 0.0)
		{
			double v = defaultValue;

			double.TryParse(properties.StringValue(name, defaultValue.ToString()), out v);

			return v;
		}


		/// <summary>
		/// Return the value of a property as a DateTime value
		/// </summary>
		/// <remarks>
		/// Performs a case-insensitive search and returns the value of the specified property as a DateTime.
		/// </remarks>
		/// <param name="properties"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static DateTime DateTimeValue(this IEnumerable<Property> properties, string name)
		{
			return properties.DateTimeValue(name, DateTime.MinValue);
		}


		/// <summary>
		/// Return the value of a property as a DateTime value
		/// </summary>
		/// <remarks>
		/// Performs a case-insensitive search and returns the value of the specified property as a DateTime.
		/// </remarks>
		/// <param name="properties"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static DateTime DateTimeValue(this IEnumerable<Property> properties, string name, DateTime defaultValue)
		{
			DateTime v = defaultValue;

			DateTime.TryParse(properties.StringValue(name, defaultValue.ToString()), out v);

			return v;
		}
	}
}
