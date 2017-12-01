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
    /// Dash service
    /// </summary>
    public interface IDashService
    {
        void Run(string[] args);
    }

    /// <summary>
    /// Dash service implementation
    /// </summary>
    public partial class DashService : ServiceBase, IDashService
    {
        // Yeah, hate this quite a bit....
        public static HttpConfiguration Config { get; set; }

        private IDisposable _webApp;
        private readonly ITestService _testService;
        private readonly IRuntime _runtime;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .MinimumLevel.Debug()
             .CreateLogger();

            var builder = new ContainerBuilder();

            builder.UseDashboardRuntime();
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterType<DashService>().As<IDashService>().InstancePerLifetimeScope();
            builder.RegisterType<TestService>().As<ITestService>().InstancePerLifetimeScope();

			var container = builder.Build();

            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);

            // Get your HttpConfiguration.
            Config = new HttpConfiguration();

            Config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			//var xml = Mock.Tests.ToXmlString();
			//var json = JsonConvert.SerializeObject(Mock.Tests);
			//var x = JsonConvert.DeserializeObject<TestDefinition[]>(json);

            container.Resolve<IDashService>().Run(args);
        }


        public DashService()
        {
            InitializeComponent();
        }

        public DashService(ITestService testService, IRuntime runtime)
            : this()
        {
            _testService = testService;
            _runtime = runtime;
        }

        public void Run(string[] args)
        {
            if (Environment.UserInteractive)
            {
                try
                {
                    OnStart(args);
                    Console.WriteLine("Service running. (key)");
                    Console.ReadKey(true);
                    Console.Write("Service stopping... ");
                    OnStop();
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
                ServiceBase[] servicesToRun = { this };
                Run(servicesToRun);
            }
        }


        protected override void OnStart(string[] args)
        {
            // @TODO: I think I'd actually rather re-build everything here (so do all the DI / container initialisation here) and tear it down in OnStop()
			//        We probably need to resolve IRuntime as our root object rather than DashService
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
