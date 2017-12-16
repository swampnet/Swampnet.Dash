using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface ITestRepository
    {
        IEnumerable<ElementDefinition> GetDefinitions();
		void Add(IEnumerable<Element> testResults);
		IEnumerable<Varient> Get(string testId, string property);
	}
}
