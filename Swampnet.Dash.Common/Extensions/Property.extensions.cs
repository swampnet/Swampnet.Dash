using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;

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

		public static TimeSpan TimeSpanValue(this IEnumerable<Property> properties, string name, TimeSpan? defaultValue = null)
		{
			TimeSpan ts;

			var s = properties.StringValue(name);
			if (string.IsNullOrEmpty(s))
			{
				if (defaultValue.HasValue)
				{
					ts = defaultValue.Value;
				}
				else
				{
					throw new ArgumentNullException(name);
				}
			}
			else
			{
				ts = XmlConvert.ToTimeSpan(s);
			}

			return ts;
		}

		public static Property Get(this IEnumerable<Property> properties, string name)
		{
			if (properties != null && properties.Any())
			{
				return properties.SingleOrDefault(x => x.Name.EqualsNoCase(name));
			}
			return null;
		}


		public static bool Exists(this IEnumerable<Property> properties, string name)
		{
			return properties != null && properties.Any(x => x.Name.EqualsNoCase(name));
		}


		public static void AddOrUpdate(this ICollection<Property> properties, string category, string name, object value)
		{
			if (properties.Exists(name))
			{
				properties.Get(name).Value = value.ToString();
			}
			else
			{
				properties.Add(new Property(category, name, value));
			}
		}

		public static void AddOrUpdate(this ICollection<Property> properties, string name, object value)
		{
			properties.AddOrUpdate(null, name, value);
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


		public static bool BoolValue(this IEnumerable<Property> properties, string name, bool defaultValue = false)
		{
			bool v = defaultValue;

			bool.TryParse(properties.StringValue(name, defaultValue.ToString()), out v);

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
