using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;

namespace UnitTests
{
	static class Mock
	{
		public static Dashboard Dashboard
		{
			get
			{
				return new Dashboard()
				{
					Groups = new List<DashboardGroup>()
					{
						new DashboardGroup()
						{

						}
					}
				};
			}
		}

		public static ElementDefinition ElementDefinition
		{
			get
			{
				return new ElementDefinition()
				{
					
				};
			}
		}

		public static IEnumerable<ElementState> GetSampleData()
		{
			DateTime now = DateTime.UtcNow;

			return new[] {
				GetState(now.AddMinutes(-0), 0.0),
				GetState(now.AddMinutes(-1), 1.0),
				GetState(now.AddMinutes(-2), 2.0),
				GetState(now.AddMinutes(-3), 3.0),
				GetState(now.AddMinutes(-4), 4.0),
				GetState(now.AddMinutes(-5), 5.0),
				GetState(now.AddMinutes(-6), 6.0),
				GetState(now.AddMinutes(-7), 7.0),
				GetState(now.AddMinutes(-8), 8.0),
				GetState(now.AddMinutes(-9), 9.0),
			};
		}


		private static int _id = 0;

		private static ElementState GetState(DateTime date, double value)
		{
			return new ElementState("test-state", ++_id)
			{
				Timestamp = date,
				Output = new List<Property>()
				{
					new Property("value", value)
				}
			};
		}
	}
}
