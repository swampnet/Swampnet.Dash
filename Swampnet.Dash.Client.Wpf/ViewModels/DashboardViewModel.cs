using Microsoft.AspNet.SignalR.Client;
using Prism.Mvvm;
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

		private ObservableCollection<string> _messages = new ObservableCollection<string>();
		private readonly string _dashName;

		public IEnumerable<string> Messages => _messages;

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

			_proxy.On("broadcastMessage", (string user, string message) =>
			{
				_uiFactory.StartNew(() =>
				{
					_messages.Add($"[{user}] {message}");
				});
			});

			_proxy.On("updateDashItems", (IEnumerable<DashItem> dashItems) =>
			{
				_uiFactory.StartNew(() =>
				{
					LastUpdate = DateTime.Now;
					foreach (var d in dashItems)
					{
						_messages.Add($"{d.Id} (" + string.Join(", ", d.Properties.Select(p => $"{p.Name}: {p.Value}")) + ")");
					}
				});
			});

			_hubConnection.Start().Wait();

			_proxy.Invoke("JoinGroup", dash);
			_dashName = dash;
		}

		public string Name => _dashName;


		public void Boosh()
		{
			_proxy.Invoke("Send", "some-name", "some-message");
		}
	}
}
