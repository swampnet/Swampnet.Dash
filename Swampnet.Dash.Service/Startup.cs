using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Cors;
using System.Web.Http;
using Swampnet.Dash.Service.Services;
using Autofac;
using Autofac.Integration.WebApi;

namespace Swampnet.Dash.Service
{
    /// <summary>
    /// Owin startup and config
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self - host.
            //var config = new HttpConfiguration();

            // Yeah, hate this quite a bit....
            DashService.Config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(DashService.Config);
            app.MapSignalR();
        }
    }
}
