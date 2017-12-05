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

		public DashboardsController(IDashboardRepository dashRepo, IState state)
        {
            _dashRepo = dashRepo;
			_state = state;
		}


        [HttpGet]
        public Task<IEnumerable<Dashboard>> Get()
        {
			return _dashRepo.GetDashboardsAsync();
        }


        [Route("dashboards/{id}")]
        [HttpGet]
        public async Task<Dashboard> Get(string id)
        {
			var dashboards = await _dashRepo.GetDashboardsAsync();

			return dashboards.Single(d => d.Id == id);
        }


		[Route("dashboards/{id}/state")]
		[HttpGet]
		public Task<IEnumerable<DashboardItem>> GetState(string id)
		{
			return _state.GetDashItemsAsync(id);
		}
	}
}
