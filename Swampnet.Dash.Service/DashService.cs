using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System.ServiceProcess;
using Serilog;
using Autofac;
using Swampnet.Dash.Service.Services;
using System.Reflection;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using System.Web.Http;
using Swampnet.Dash.Common.Interfaces;
using Swampnet.Dash.Common;
using Newtonsoft.Json;
using Swampnet.Dash.Common.Entities;

namespace Swampnet.Dash.Service
{
    /// <summary>
    /// Dash service implementation
    /// </summary>
    public partial class DashService : ServiceBase
    {
        // Yeah, hate this quite a bit....
        public static HttpConfiguration Config { get; set; }

        private IDisposable _webApp;
        private IRuntime _runtime;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .MinimumLevel.Debug()
             .CreateLogger();

            var dashService = new DashService();

            if (Environment.UserInteractive)
            {
                try
                {
                    dashService.OnStart(args);
                    Console.WriteLine("Service running. (key)");
                    Console.ReadKey(true);
                    Console.Write("Service stopping... ");
                    dashService.OnStop();
                    Console.WriteLine("Service stopped.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("(key)");
                    Console.WriteLine(ex.ToString());
                    Console.ReadKey(true);
                }
            }
            else
            {
                ServiceBase[] servicesToRun = { dashService };
                Run(servicesToRun);
            }
        }


        public DashService()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.UseDashboardRuntime();
            builder.UseDAL();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterType<Broadcast>().As<IBroadcast>();

            var container = builder.Build();

            _runtime = container.Resolve<IRuntime>();

            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);

            // Get your HttpConfiguration.
            Config = new HttpConfiguration();

            Config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            //var xml = Mock.Tests.ToXmlString();
            //var json = JsonConvert.SerializeObject(Mock.Tests);
            //var x = JsonConvert.DeserializeObject<TestDefinition[]>(json);

            _runtime.Start();

            var url = "http://localhost:8080/";
            _webApp = WebApp.Start<Startup>(url);

            Log.Information("Server running on {url}", url);
        }


        protected override void OnStop()
        {
            _runtime.Stop();

            if (_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
                Log.Information("Server stopped");
            }
        }
    }
}
