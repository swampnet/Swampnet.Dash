using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash
{
	public static class ObjectExtensions
	{
		public static string ToXmlString(this object o)
		{
			try
			{
				string xml = o as string;

				if (xml == null)
				{
					if (o != null)
					{
						var s = new XmlSerializer(o.GetType());

						using (var sw = new StringWriter())
						{
							s.Serialize(sw, o);
							xml = sw.ToString();
						}
					}
					else
					{
						xml = "<null/>";
					}
				}

				return xml;
			}
			catch (Exception ex)
			{
				//ex.AddData("o", o == null ? "null" : o.GetType().Name);
				throw;
			}
		}
	}
}
