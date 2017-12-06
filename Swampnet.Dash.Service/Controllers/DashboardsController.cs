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
		private readonly IState _state;
        private readonly IArgosRunner _argosRunner;

        public DashboardsController(IDashboardRepository dashRepo, IState state, IArgosRunner argosRunner)
        {
            _dashRepo = dashRepo;
			_state = state;
            _argosRunner = argosRunner;
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
			var dashboards = await _dashRepo.GetDashboardsAsync();

			return Ok(dashboards.Single(d => d.Id == id));
        }

		
		[Route("dashboards/{id}/state")]
		[HttpGet]
		public async Task<IHttpActionResult> GetState(string id)
		{
            List<DashboardItem> states = new List<DashboardItem>();

            var dashboard = await _dashRepo.GetDashboardAsync(id);

            if(dashboard == null)
            {
                return NotFound();
            }

            // Get all tests for dashboard
            if (dashboard.Tests != null)
			{
				states.AddRange(await _state.GetDashItemsAsync(dashboard.Tests.Select(t => t.Id)));
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
