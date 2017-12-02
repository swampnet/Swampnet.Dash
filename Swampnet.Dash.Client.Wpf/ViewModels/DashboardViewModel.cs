using Microsoft.AspNet.SignalR.Client;
using Prism.Mvvm;
using Serilog;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Client.Wpf.ViewModels
{
	class DashboardViewModel : BindableBase
	{
		private readonly HubConnection _hubConnection;
		private readonly IHubProxy _proxy;
		private readonly TaskFactory _uiFactory;

		private ObservableCollection<DashItemViewModel> _messages = new ObservableCollection<DashItemViewModel>();
		private readonly string _dashName;

		public IEnumerable<DashItemViewModel> Messages => _messages;

		private DateTime _lastUpdate;

		public DateTime LastUpdate
		{
			get { return _lastUpdate; }
			set { SetProperty(ref _lastUpdate, value); }
		}


		public DashboardViewModel(string dash)
		{
			LastUpdate = DateTime.Now;

			// Construct a TaskFactory that uses the UI thread's context
			_uiFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
			_hubConnection = new HubConnection("http://localhost:8080/");
			_proxy = _hubConnection.CreateHubProxy("DashboardHub");

			_proxy.On("UpdateDash", (IEnumerable<DashItem> dashItems) => UpdateDash(dashItems));

			_hubConnection.Start().Wait();

			_proxy.Invoke("JoinGroup", dash);
			_dashName = dash;
		}

		public string Name => _dashName;


		public void Boosh()
		{
			_proxy.Invoke("Send", "some-name", "some-message");
		}

		private void UpdateDash(IEnumerable<DashItem> dashItems)
		{
			_uiFactory.StartNew(() =>
			{
				LastUpdate = DateTime.Now;
				foreach (var di in dashItems)
				{
					var dashItem = _messages.SingleOrDefault(d => d.Id == di.Id);
					if (dashItem == null)
					{
						dashItem = new DashItemViewModel(null);
						_messages.Add(dashItem);
					}
					dashItem.Update(di);
				}
			});
		}
	}
}
