using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface ITest
    {
		Task<Element> RunAsync(ElementDefinition testDefinition);
		TestMeta Meta { get; }
	}
}
