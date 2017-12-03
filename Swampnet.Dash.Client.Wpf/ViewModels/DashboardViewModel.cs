﻿using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Prism.Mvvm;
using Serilog;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Client.Wpf.ViewModels
{
	class DashboardViewModel : BindableBase
	{
		private readonly HubConnection _hubConnection;
		private readonly IHubProxy _proxy;
		private readonly TaskFactory _uiFactory;

		private readonly ObservableCollection<DashItemViewModel> _messages = new ObservableCollection<DashItemViewModel>();
		private readonly string _dashName;
		private DashMetaData _metaData;

		public IEnumerable<DashItemViewModel> Messages => _messages;


		private DateTime _lastUpdate;

		public DateTime LastUpdate
		{
			get { return _lastUpdate; }
			set { SetProperty(ref _lastUpdate, value); }
		}


		public DashboardViewModel(string dash)
		{
			// Construct a TaskFactory that uses the UI thread's context
			_uiFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
			_hubConnection = new HubConnection("http://localhost:8080/");
			_proxy = _hubConnection.CreateHubProxy("DashboardHub");
			_dashName = dash;

			InitialiseDash(dash);
		}

		public string Name => _metaData?.Name;
		public string Description => _metaData?.Description;


		public void Boosh()
		{
			_proxy.Invoke("Send", "some-name", "some-message");
		}

		private void UpdateDash(IEnumerable<DashItem> dashItems)
		{
			LastUpdate = DateTime.Now;

			_uiFactory.StartNew(() =>
			{
				LastUpdate = DateTime.Now;
				foreach (var di in dashItems)
				{
					var dashItem = _messages.SingleOrDefault(d => d.Id == di.Id);
					if (dashItem == null)
					{
						// @TODO: Figure out a better way of resolving meta-data. It won't always be id based (ie, in Argos mode the meta data is the same for all items)
						var meta = _metaData.Items.Single(x => x.Id == di.Id);
						dashItem = new DashItemViewModel(meta);
						_messages.Add(dashItem);
					}
					dashItem.Update(di);
				}
			});
		}



		private async void InitialiseDash(string dashId)
		{
			LastUpdate = DateTime.Now;

			_metaData = await Api.GetAsync<DashMetaData>($"dashboards/{dashId}/meta");

			foreach(var item in _metaData.Items)
			{
				_messages.Add(new DashItemViewModel(item));
			}

			await _hubConnection.Start()
					.ContinueWith((x) => _proxy.Invoke("JoinGroup", dashId))
					.ContinueWith((x) => _proxy.On("UpdateDash", (IEnumerable<DashItem> dashItems) => UpdateDash(dashItems)))
					.ContinueWith((x) => RaisePropertyChanged(""))
					;
		}
	}
}