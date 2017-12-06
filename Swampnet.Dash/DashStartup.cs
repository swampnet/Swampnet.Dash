using Autofac;
using Swampnet.Dash.Common.Interfaces;
using Swampnet.Dash.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash
{
    public static class DashStartup
    {
        // @TODO: Don't we have some kind of abstraction layer of all this stuff?
        public static void UseDashboardRuntime(this ContainerBuilder builder)
        {
            builder.RegisterType<Runtime>().As<IRuntime>();
            builder.RegisterType<ArgosRepository>().As<IArgosRepository>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardRepository>().As<IDashboardRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TestRepository>().As<ITestRepository>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(DashStartup).Assembly).As<ITest>().SingleInstance();
            builder.RegisterAssemblyTypes(typeof(DashStartup).Assembly).As<IArgos>().SingleInstance();
            builder.RegisterType<TestRunner>().As<ITestRunner>().SingleInstance();
			builder.RegisterType<ArgosRunner>().As<IArgosRunner>().SingleInstance();
        }
    }
}
