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

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .CreateLogger();

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterType<DashService>().As<IDashService>().InstancePerLifetimeScope();
            builder.RegisterType<TestService>().As<ITestService>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardRepository>().As<IDashboardRepository>().InstancePerLifetimeScope();

            var container = builder.Build();

            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);

            // Get your HttpConfiguration.
            Config = new HttpConfiguration();

            Config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            container.Resolve<IDashService>().Run(args);
        }


        public DashService()
        {
            InitializeComponent();
        }

        public DashService(ITestService testService)
            : this()
        {
            _testService = testService;
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
            var url = "http://localhost:8080/";
            _webApp = WebApp.Start<Startup>(url);
            Log.Information("Server running on {url}", url);
        }


        protected override void OnStop()
        {
            if(_webApp != null)
            {
                _webApp.Dispose();
                _webApp = null;
                Log.Information("Server stopped");
            }
        }
    }
}
