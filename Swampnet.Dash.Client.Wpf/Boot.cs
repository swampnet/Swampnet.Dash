using CommonServiceLocator;
using Prism.Autofac;
using Swampnet.Dash.Client.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Swampnet.Dash.Client.Wpf
{
	class Boot : AutofacBootstrapper
	{
		protected override DependencyObject CreateShell()
		{
			return ServiceLocator.Current.GetInstance<MainWindow>();
		}

		protected override void InitializeShell()
		{
			Application.Current.MainWindow.Show();
		}
	}
}
