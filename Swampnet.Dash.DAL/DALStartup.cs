using Autofac;
using Swampnet.Dash.Common.Interfaces;
using Swampnet.Dash.DAL;
using Swampnet.Dash.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash
{
    public static class DALStartup
    {
        // @TODO: Don't we have some kind of abstraction layer of all this stuff?
        public static void UseDAL(this ContainerBuilder builder)
        {
            builder.RegisterType<ArgosRepository>().As<IArgosRepository>().InstancePerLifetimeScope();
            builder.RegisterType<DashboardRepository>().As<IDashboardRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TestRepository>().As<ITestRepository>().InstancePerLifetimeScope();
			builder.RegisterType<ValuesRepository>().As<IValuesRepository>().InstancePerLifetimeScope();
		}
    }
}
