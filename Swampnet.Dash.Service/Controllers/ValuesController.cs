using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Swampnet.Dash.Service.Controllers
{
	public class ValuesController : ApiController
	{
		private readonly IValuesRepository _valuesRepository;

		public ValuesController(IValuesRepository valuesRepository)
		{
			_valuesRepository = valuesRepository;
		}


		[Route("values/{itemDefinitionId}/{propertyName}")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string itemDefinitionId, string propertyName, [FromUri]int seconds)
		{
			var results = await _valuesRepository.GetHistory(itemDefinitionId, propertyName, TimeSpan.FromSeconds(seconds));

			return Ok(results);
		}
	}
}
