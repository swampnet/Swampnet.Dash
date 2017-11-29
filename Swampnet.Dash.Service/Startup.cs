using Owin;
using Microsoft.Owin.Cors;
using System.Web.Http;
using System.Net.Http.Headers;

namespace Swampnet.Dash.Service
{
    /// <summary>
    /// Owin startup and config
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Yeah, hate this quite a bit....
            var config = DashService.Config;// new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
            app.MapSignalR();
        }
    }
}
