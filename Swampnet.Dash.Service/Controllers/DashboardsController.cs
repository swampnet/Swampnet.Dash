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
    public class DashboardsController : ApiController
    {
        private readonly IDashboardRepository _dashRepo;
        private readonly IArgosRunner _argosRunner;
        private readonly ITestRunner _testRunner;

        public DashboardsController(IDashboardRepository dashRepo, ITestRunner testRunner, IArgosRunner argosRunner)
        {
            _dashRepo = dashRepo;
            _argosRunner = argosRunner;
            _testRunner = testRunner;
        }


        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
			var dashboards = await _dashRepo.GetDashboardsAsync();

			return Ok(dashboards);
        }


        [Route("dashboards/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
			var dashboard = await _dashRepo.GetDashboardAsync(id);

			if (dashboard == null)
			{
				return NotFound();
			}

			return Ok(dashboard);
        }

		
		[Route("dashboards/{id}/state")]
		[HttpGet]
		public async Task<IHttpActionResult> GetState(string id)
		{
            // @TODO: Yeah, way too much going on in here. Farm all this off to another service or something.
            var states = new List<ElementState>();

            var dashboard = await _dashRepo.GetDashboardAsync(id);

            if(dashboard == null)
            {
                return NotFound();
            }

            // Get all tests for dashboard
            if (dashboard.Tests != null)
			{
				states.AddRange(_testRunner.GetTestResults(dashboard.Tests.Select(t => t.Id)).Select(x => new ElementState(x.Id, x.Id)
                {
                    Output = x.Output,
                    Status = x.Status,
                    Timestamp = x.Timestamp                    
                }));
			}

            // Sort out the argos stuff
            if(dashboard.Argos != null)
            {
                foreach(var a in dashboard.Argos)
                {
                    var rs = await _argosRunner.GetState(a.Id);
                    if(rs != null)
                    {
                        states.AddRange(rs.Items);
                    }
                }
            }

			return Ok(states);
		}
	}
}
