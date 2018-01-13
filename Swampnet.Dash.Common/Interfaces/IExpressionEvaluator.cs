using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IExpressionEvaluator
    {
		bool Evaluate(Expression expression, ElementState state);
	}
}
