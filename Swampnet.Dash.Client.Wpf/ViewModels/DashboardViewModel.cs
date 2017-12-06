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
    /// <summary>
    /// @TODO: A lot of this stuff (ie, all the SignalR stuff) should probably be pulled out into a separate class/service
    /// </summary>
	class DashboardViewModel : BindableBase, IDisposable
	{
        private const string UPDATE_MESSAGE = "Update";
        private const string REFRESH_MESSAGE = "Refresh";

        private readonly HubConnection _hubConnection;
		private readonly IHubProxy _proxy;
		private readonly TaskFactory _uiFactory;

		private readonly ObservableCollection<DashItemViewModel> _items = new ObservableCollection<DashItemViewModel>();
		private readonly string _dashName;
		private Dashboard _dashboard;

		public IEnumerable<DashItemViewModel> Items => _items;


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

		public string Name => _dashboard?.Id;
		public string Description => _dashboard?.Description;


		public void Boosh()
		{
			_proxy.Invoke("Send", "some-name", "some-message");
		}

        /// <summary>
        /// Update / Add DashItems based on source
        /// </summary>
        private void Update(IEnumerable<DashboardItem> source)
        {
            foreach (var di in source)
            {
                var dashItem = _items.SingleOrDefault(d => d.Id == di.Id);
                if (dashItem == null)
                {
                    IEnumerable<Meta> metaData = null;
                    var test = _dashboard.Tests?.SingleOrDefault(t => t.TestId == di.Id);
                    if (test == null)
                    {
                        metaData = _dashboard.DefaultMetaData;
                    }
                    else
                    {
                        metaData = test.MetaData;
                    }
                    dashItem = new DashItemViewModel(di.Id, metaData);
                    _items.Add(dashItem);
                }
                dashItem.Update(di);
            }

        }

        /// <summary>
        /// Remove any DashItems that do not appear in source
        /// </summary>
        private void Remove(IEnumerable<DashboardItem> source)
        {
            foreach (var di in _items.Where(i => !source.Select(s => s.Id).Contains(i.Id)).ToArray())
            {
                _items.Remove(di);
            }
        }


        private IEnumerable<Meta> GetMetaData(DashboardItem di)
        {
            IEnumerable<Meta> metaData;

            // Look for a test with the same id
            var test = _dashboard.Tests?.SingleOrDefault(t => t.TestId == di.Id);
            if (test != null)
            {
                metaData = test.MetaData;
            }

            // No test going by that id - use generic metadata
            else
            {
                metaData = _dashboard.DefaultMetaData;
            }

            return metaData;
        }

        private async void InitialiseDash(string dashId)
		{
			LastUpdate = DateTime.Now;

			await UpdateMetaData(dashId);

			var dashItems = await Api.GetDashState(dashId);

            // Get initial data
            Update(dashItems);

            _hubConnection.Closed += HubConnectionClosed;
            _hubConnection.ConnectionSlow += HubConnectionConnectionSlow;
            _hubConnection.Error += HubConnectionError;
            _hubConnection.Received += HubConnectionReceived;
            _hubConnection.Reconnected += HubConnectionReconnected;
            _hubConnection.Reconnecting += HubConnectionReconnecting;
            _hubConnection.StateChanged += HubConnectionStateChanged;

            _proxy.On(UPDATE_MESSAGE, (IEnumerable<DashboardItem> di) => _uiFactory.StartNew(() =>
            {
                LastUpdate = DateTime.Now;
                Update(di);
            }));

            _proxy.On(REFRESH_MESSAGE, (IEnumerable<DashboardItem> di) => _uiFactory.StartNew(() =>
            {
                LastUpdate = DateTime.Now;
                Update(di);
                Remove(di);
            }));


            await _hubConnection.Start()
					.ContinueWith((x) => _proxy.Invoke("JoinGroup", dashId))
                    ;
        }


        private async Task UpdateMetaData(string dashId)
		{
			_dashboard = await Api.GetDashboard(dashId);
			RaisePropertyChanged("");

			if(_dashboard.Tests != null && _dashboard.Tests.Any())
			{
				foreach (var item in _dashboard.Tests)
				{
					_items.Add(new DashItemViewModel(item.TestId, item.MetaData));
				}
			}
		}

        #region SignalR Events

        private void HubConnectionStateChanged(StateChange obj)
        {
            Log.Debug("HubConnectionStateChanged {oldState} -> {newState}", obj.OldState, obj.NewState);
        }

        private void HubConnectionReconnecting()
        {
            Log.Debug("HubConnectionReconnecting");
        }

        private void HubConnectionReconnected()
        {
            Log.Debug("HubConnectionReconnected");
        }

        private void HubConnectionReceived(string obj)
        {
            Log.Debug("HubConnectionReceived"); // 'obj' is the entire message
        }

        private void HubConnectionError(Exception ex)
        {
            Log.Error(ex, "HubConnectionError");
        }

        private void HubConnectionConnectionSlow()
        {
            Log.Debug("HubConnectionConnectionSlow");
        }

        private void HubConnectionClosed()
        {
            Log.Debug("HubConnectionClosed");
        }

        #endregion

        public void Dispose()
        {
            if (_hubConnection != null)
            {
                _hubConnection.Closed -= HubConnectionClosed;
                _hubConnection.ConnectionSlow -= HubConnectionConnectionSlow;
                _hubConnection.Error -= HubConnectionError;
                _hubConnection.Received -= HubConnectionReceived;
                _hubConnection.Reconnected -= HubConnectionReconnected;
                _hubConnection.Reconnecting -= HubConnectionReconnecting;
                _hubConnection.StateChanged -= HubConnectionStateChanged;

                _hubConnection.Dispose();
            }
        }
    }
}
