using Microsoft.AspNet.SignalR.Client;
using Prism.Mvvm;
using Swampnet.Dash.Client.Wpf.ViewModels;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Client.Wpf
{
    class MainViewModel : BindableBase
    {
		private ObservableCollection<DashboardViewModel> _dashboards = new ObservableCollection<DashboardViewModel>();

		public MainViewModel()
		{
			_dashboards.Add(new DashboardViewModel("dash-01"));
			//_dashboards.Add(new DashboardViewModel("dash-02"));
			//_dashboards.Add(new DashboardViewModel("dash-02"));
			//_dashboards.Add(new DashboardViewModel("local-dash"));
			_dashboards.Add(new DashboardViewModel("argos-test"));
		}

		public IEnumerable<DashboardViewModel> Dashboards => _dashboards;
	}
}
