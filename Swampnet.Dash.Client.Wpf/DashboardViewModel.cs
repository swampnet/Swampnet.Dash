using Microsoft.AspNet.SignalR.Client;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Client.Wpf
{
	class DashboardViewModel
	{
		private HubConnection _hubConnection;
		private readonly IHubProxy _proxy;
		private TaskFactory _uiFactory;

		private ObservableCollection<string> _messages = new ObservableCollection<string>();
		private readonly string _dash;

		public IEnumerable<string> Messages => _messages;


		public DashboardViewModel(string dash)
		{
			// Construct a TaskFactory that uses the UI thread's context
			_uiFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

			_hubConnection = new HubConnection("http://localhost:8080/");
			_proxy = _hubConnection.CreateHubProxy("DashboardHub");

			_proxy.On("broadcastMessage", (string x, string y) =>
			{
				_uiFactory.StartNew(() =>
				{
					_messages.Add($"[{x}] {y}");
				});
			});

			_proxy.On("updateDashItems", (IEnumerable<DashItem> dashItems) =>
			{
				_uiFactory.StartNew(() =>
				{
					foreach (var d in dashItems)
					{
						_messages.Add($"{d.Id} (" + string.Join(", ", d.Properties.Select(p => $"{p.Name}: {p.Value}")) + ")");
					}
				});
			});

			_hubConnection.Start().Wait();

			_proxy.Invoke("JoinGroup", dash);
			_dash = dash;
		}

		public string Name => _dash;


		public void Boosh()
		{
			_proxy.Invoke("Send", "some-name", "some-message");
		}
	}
}
