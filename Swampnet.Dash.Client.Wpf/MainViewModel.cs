using Microsoft.AspNet.SignalR.Client;
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
    class MainViewModel
    {
		public MainViewModel()
		{
			Lefty = new DashboardViewModel("dash-01");
			Righty = new DashboardViewModel("dash-02");
		}
		public DashboardViewModel Lefty { get; set; }
		public DashboardViewModel Righty { get; set; }
	}
}
