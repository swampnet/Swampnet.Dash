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
        public string Get()
        {
            return "get";
        }


        [Route("dashboards/{dashId}")]
        [HttpGet]
        public string Get(string dashId)
        {
            return $"get {dashId}";
        }


        [Route("dashboards/{dashId}/meta")]
        [HttpGet]
        public ClientMeta GetMeta(string dashId)
        {
            return _dashRepo.GetMetaData(dashId);
        }
    }
}
