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
		private readonly IEnumerable<ITest> _tests;

		public TestsController(IEnumerable<ITest> tests)
        {
			_tests = tests;
		}


        public IEnumerable<TestMeta> Get()
        {
			return _tests.Select(t => t.Meta);
        }
    }
}
