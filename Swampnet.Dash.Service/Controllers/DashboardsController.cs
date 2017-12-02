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

        public DashboardsController(IDashboardRepository dashRepo)
        {
            _dashRepo = dashRepo;
        }


        [HttpGet]
        public async Task<IEnumerable<Dashboard>> Get()
        {
			var dashboards = await _dashRepo.GetActiveDashboardsAsync();

			return dashboards;
        }


        [Route("dashboards/{name}")]
        [HttpGet]
        public async Task<Dashboard> Get(string name)
        {
			var dashboards = await _dashRepo.GetActiveDashboardsAsync();

			return dashboards.Single(d => d.Name == name);
        }


        [Route("dashboards/{dashId}/meta")]
        [HttpGet]
        public async Task<DashMetaData> GetMeta(string dashId)
        {
			var meta = await _dashRepo.GetMetaDataAsync(dashId);
			return meta;
        }
    }
}
