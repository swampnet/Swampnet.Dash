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
	}
}
