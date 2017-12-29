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
        private readonly DashboardHub _hub;
		private readonly TaskFactory _uiFactory;

		private Dashboard _dashboard;
        private ObservableCollection<DashboardGroupViewModel> _groups;
        private IEnumerable<DashboardGroupViewModel> _defaultGroups;
        private DateTime _lastUpdate;
        private bool _loadingDashboard;


        public DashboardViewModel(string dash)
		{
            _dashName = dash;

            // Construct a TaskFactory that uses the UI thread's context
            _uiFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            _hub = new DashboardHub(dash);

            InitialiseDash(dash);
		}

        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { SetProperty(ref _lastUpdate, value); }
        }

        public bool LoadingDashboard
        {
            get { return _loadingDashboard; }
            set { SetProperty(ref _loadingDashboard, value); }
        }


        public IEnumerable<DashboardGroupViewModel> Groups => _groups;
        public string Id => _dashboard?.Id;
		public string Description => _dashboard?.Description;
        public string Template => _dashboard?.Template;

        /// <summary>
        /// Update / Add DashItems based on source. Optionally remove those not in the list.
        /// </summary>
        private void Update(IEnumerable<ElementState> source, bool isFullRefresh = false)
        {
            foreach (var elementState in source)
            {
				var definition = GetElementDefinition(elementState.ElementId);

				// @todo: Having multiple test definitions with same test id on same dash breaks this
                var item = _groups.SelectMany(g => g.ElementStates).Where(d => d.Id == elementState.Id).Distinct().SingleOrDefault();
                if (item == null)
                {
                    IEnumerable<Map> metaData = null;
                    var test = _dashboard.Tests?.SingleOrDefault(t => t.Id == elementState.Id);
                    if (test == null)
                    {
                        metaData = _dashboard.Mapping;
                    }
                    else
                    {
                        metaData = test.Mapping;
                    }
                    item = new ElementStateViewModel(_dashboard, definition, elementState.Id);
                }

                item.Update(elementState);

                AssignGroup(item);
            }

            if (isFullRefresh)
            {
                // Remove any DashItems that do not appear in source
                foreach (var id in _groups.SelectMany(g => g.ElementStates).Where(i => !source.Select(s => s.Id).Contains(i.Id)).Select(x => x.Id).ToArray())
                {
                    foreach (var group in _groups)
                    {
                        group.ElementStates.Remove(group.ElementStates.SingleOrDefault(i => i.Id == id));
                    }
                }
            }

            LastUpdate = DateTime.Now;
        }

		private ElementDefinition GetElementDefinition(string id)
		{
			var itemDefinition = _dashboard.Tests.SingleOrDefault(t => t.Id == id);
			if(itemDefinition == null)
			{
				itemDefinition = _dashboard.Argos.SingleOrDefault(t => t.Id == id);
			}

			if(itemDefinition == null)
			{
				System.Diagnostics.Debugger.Break();
			}

			return itemDefinition;
		}


		/// <summary>
		/// Add item to a group (or groups)
		/// </summary>
		/// <param name="item"></param>
		private void AssignGroup(ElementStateViewModel item)
        {
            var targetGroups = GetTargetGroups(item);

            // Remove from any groups this item already exists in (excluding the 'target' groups)
            foreach(var group in _groups.Where(g => !targetGroups.Contains(g) && g.ElementStates.Select(i => i.Id).Contains(item.Id)))
            {
                group.ElementStates.Remove(item);
            }

            // Add to target groups
            foreach (var group in targetGroups)
            {
                if (!group.ElementStates.Contains(item))
                {
                    group.ElementStates.Add(item);
                }
            }
        }


        /// <summary>
        /// Figure out the group(s) this item belongs in
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private IEnumerable<DashboardGroupViewModel> GetTargetGroups(ElementStateViewModel item)
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


        // @todo: So this is quite a chunky method. We can probably split it up a bit.
        private async void InitialiseDash(string dashId)
		{
            try
            {
                LoadingDashboard = true;

                // Load dashboard meta data
                _dashboard = await Api.GetDashboard(dashId);

                // Create groups
                _groups = new ObservableCollection<DashboardGroupViewModel>(_dashboard.Groups?.Select(g => new DashboardGroupViewModel(g)));

                // Add a default group if none defined
                if (!_groups.Any())
                {
                    _groups.Add(new DashboardGroupViewModel(new DashboardGroup()
                    {
                        Id = "_default",
                        Title = "default",
                        IsDefault = true
                    }));
                }

                // Figure out default groups
                _defaultGroups = _groups.Where(g => g.IsDefault);
                if (!_defaultGroups.Any())
                {
                    _defaultGroups = _groups.Take(1);
                }

                // Set up known dashboard items
                if (_dashboard.Tests != null && _dashboard.Tests.Any())
                {
                    foreach (var item in _dashboard.Tests)
                    {
                        AssignGroup(new ElementStateViewModel(_dashboard, item, item.Id));
                        //_groups.First().Items.Add(new DashboardItemViewModel(item.Id, item.MetaData));
                    }
                }

                // Get initial data
                var dashItems = await Api.GetDashState(dashId);
                Update(dashItems);

                _hub.Proxy.On(UPDATE_MESSAGE, (IEnumerable<ElementState> di) => _uiFactory.StartNew(() =>
                {
                    Update(di, false);
                }));

                _hub.Proxy.On(REFRESH_MESSAGE, (IEnumerable<ElementState> di) => _uiFactory.StartNew(() =>
                {
                    Update(di, true);
                }));

                _hub.Start();

                LastUpdate = DateTime.Now;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            finally
            {
                LoadingDashboard = false;

				// Refresh UI
				RaisePropertyChanged("");
			}
		}



        public void Dispose()
        {
            _hub?.Dispose();
        }
    }
}
