using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Autofac;
using Swampnet.Dash.Service.Services;

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
        private IDisposable _webApp;
        private readonly ITestService _testService;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .CreateLogger();

            var builder = new ContainerBuilder();

            builder.RegisterType<DashService>().As<IDashService>();
            builder.RegisterType<TestService>().As<ITestService>();

            var container = builder.Build();

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
