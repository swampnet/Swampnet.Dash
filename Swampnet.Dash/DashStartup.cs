using Autofac;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using Swampnet.Dash.Services;
using Swampnet.Dash.Tests;
using Swampnet.Rules;
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
			builder.RegisterAssemblyTypes(typeof(DashStartup).Assembly).AssignableTo<ITest>();
			builder.RegisterAssemblyTypes(typeof(DashStartup).Assembly).AssignableTo<IArgos>();
            builder.RegisterType<TestRunner>().As<ITestRunner>().SingleInstance();
			builder.RegisterType<ArgosRunner>().As<IArgosRunner>().SingleInstance();
            builder.RegisterType<Services.RuleProcessor>().As<IRuleProcessor>().SingleInstance();
			builder.RegisterType<StateProcessor>().As<IStateProcessor>().SingleInstance();			
			builder.RegisterType<Analysis>().As<IAnalysis>().SingleInstance();
        }
    }
}
