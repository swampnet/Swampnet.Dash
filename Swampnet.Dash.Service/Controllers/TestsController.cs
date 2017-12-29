using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using Swampnet.Dash.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Swampnet.Dash.Service.Controllers
{
    public class TestsController : ApiController
    {
		private readonly ITestRepository _testRepo;
		private readonly IEnumerable<ITest> _tests;
		private readonly IValuesRepository _valuesRepository;

		public TestsController(ITestRepository testRepo, IEnumerable<ITest> tests, IValuesRepository valuesRepository)
        {
			_testRepo = testRepo;
			_tests = tests;
			_valuesRepository = valuesRepository;
		}


        public IHttpActionResult Get()
        {
			return Ok(_testRepo.GetDefinitions());
        }

		public IHttpActionResult Get(string id)
		{
			return Ok(_testRepo.GetDefinitions().Single(d => d.Id == id));
		}


		[Route("tests/{testId}/{propertyName}")]
		[HttpGet]
		public async Task<IHttpActionResult> GetHistory(string testId, string propertyName, [FromUri]int? seconds)
		{
			var data = await _valuesRepository.GetHistory(testId, propertyName, seconds.HasValue ? TimeSpan.FromSeconds(seconds.Value) : TimeSpan.MaxValue);

			return Ok(data);
		}
	}
}
