using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
	public interface IValuesRepository
	{
		Task Add(IEnumerable<ElementState> elements);
		Task<IEnumerable<Varient>> GetHistory(string itemDefinitionId, string propertyName, TimeSpan history);
	}
}
