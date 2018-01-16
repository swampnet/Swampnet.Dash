using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
	public interface IAnalysis
	{
		double Avg(ElementState state, string propertyName, TimeSpan history, bool isRolling);
		double StdDev(ElementState state, string propertyName, TimeSpan history, bool isSample);
		object Last(ElementState state, string propertyName);
	}
}
