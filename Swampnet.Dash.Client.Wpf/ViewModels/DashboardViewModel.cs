using Microsoft.AspNet.SignalR.Client;
using Prism.Mvvm;
using Serilog;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private readonly string _dashName;
        private readonly HubConnection _hubConnection;
		private readonly IHubProxy _proxy;
		private readonly TaskFactory _uiFactory;

		private Dashboard _dashboard;
        private ObservableCollection<DashboardGroupViewModel> _groups;
        private IEnumerable<DashboardGroupViewModel> _defaultGroups;
        private DateTime _lastUpdate;


        public DashboardViewModel(string dash)
		{
            _dashName = dash;

            // Construct a TaskFactory that uses the UI thread's context
            _uiFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
			_hubConnection = new HubConnection("http://localhost:8080/");
			_proxy = _hubConnection.CreateHubProxy("DashboardHub");

			InitialiseDash(dash);
		}

        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { SetProperty(ref _lastUpdate, value); }
        }


        public IEnumerable<DashboardGroupViewModel> Groups => _groups;
        public string Id => _dashboard?.Id;
		public string Description => _dashboard?.Description;


        /// <summary>
        /// Update / Add DashItems based on source
        /// </summary>
        private void Update(IEnumerable<DashboardItem> source, bool isFullRefresh = false)
        {
            foreach (var di in source)
            {
                var item = _groups.SelectMany(g => g.Items).Where(d => d.Id == di.Id).Distinct().SingleOrDefault();
                if (item == null)
                {
                    IEnumerable<Meta> metaData = null;
                    var test = _dashboard.Tests?.SingleOrDefault(t => t.Id == di.Id);
                    if (test == null)
                    {
                        metaData = _dashboard.DefaultMetaData;
                    }
                    else
                    {
                        metaData = test.MetaData;
                    }
                    item = new DashboardItemViewModel(di.Id, metaData);
                }

                item.Update(di);

                AssignGroup(item);
            }

            if (isFullRefresh)
            {
                // Remove any DashItems that do not appear in source
                foreach (var id in _groups.SelectMany(g => g.Items).Where(i => !source.Select(s => s.Id).Contains(i.Id)).Select(x => x.Id).ToArray())
                {
                    foreach (var group in _groups)
                    {
                        group.Items.Remove(group.Items.SingleOrDefault(i => i.Id == id));
                    }
                }
            }

            LastUpdate = DateTime.Now;
        }


        /// <summary>
        /// Add item to a group (or groups)
        /// </summary>
        /// <param name="item"></param>
        private void AssignGroup(DashboardItemViewModel item)
        {
            var targetGroups = GetTargetGroups(item);

            // Remove from any groups this item already exists in (excluding the 'target' groups)
            foreach(var group in _groups.Where(g => !targetGroups.Contains(g) && g.Items.Select(i => i.Id).Contains(item.Id)))
            {
                group.Items.Remove(item);
            }

            // Add to target groups
            foreach (var group in targetGroups)
            {
                if (!group.Items.Contains(item))
                {
                    group.Items.Add(item);
                }
            }
        }


        /// <summary>
        /// Figure out the group(s) this item belongs in
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private IEnumerable<DashboardGroupViewModel> GetTargetGroups(DashboardItemViewModel item)
        {
            IEnumerable<DashboardGroupViewModel> targets = null;

            if (!string.IsNullOrEmpty(item.Group))
            {
                targets = _groups.Where(g => g.Id == item.Group);
            }

            if(targets == null || !targets.Any())
            {
                targets = _defaultGroups;
            }

            return targets;
        }


        /// <summary>
        /// Get MetaData for this item
        /// </summary>
        private IEnumerable<Meta> GetMetaData(DashboardItem item)
        {
            // @TODO: There's a heirachy here:
            //  Test / Argos specific
            //  Group specific
            //  Default

            IEnumerable<Meta> metaData;

            // Look for a test with the same id
            var test = _dashboard.Tests?.SingleOrDefault(t => t.Id == item.Id);
            if (test != null)
            {
                metaData = test.MetaData;
            }

            
            // No test going by that id - use default metadata
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
                Update(di, false);
            }));

            _proxy.On(REFRESH_MESSAGE, (IEnumerable<DashboardItem> di) => _uiFactory.StartNew(() =>
            {
                Update(di, true);
            }));

            // Connect to server and join a group based on the dashboard id
            await _hubConnection.Start()
					.ContinueWith((x) => _proxy.Invoke("JoinGroup", dashId))
                    ;
        }


        private async Task UpdateMetaData(string dashId)
		{
			_dashboard = await Api.GetDashboard(dashId);
            _groups = new ObservableCollection<DashboardGroupViewModel>(_dashboard.Groups.Select(g => new DashboardGroupViewModel(g))); //@todo: Need a default if no group defined

            _defaultGroups = _groups.Where(g => g.IsDefault);
            if (!_defaultGroups.Any())
            {
                _defaultGroups = _groups.Take(1);
            }

            RaisePropertyChanged("");

            // Set up known dashboard items
			if(_dashboard.Tests != null && _dashboard.Tests.Any())
			{
				foreach (var item in _dashboard.Tests)
				{
                    _groups.First().Items.Add(new DashboardItemViewModel(item.Id, item.MetaData));
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
