using Prism.Mvvm;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Swampnet.Dash.Client.Wpf.ViewModels
{
    /// <summary>
    /// Group view model
    /// </summary>
    class DashboardGroupViewModel : BindableBase
    {
        private readonly ObservableCollection<ElementStateViewModel> _elementStates = new ObservableCollection<ElementStateViewModel>();
        private readonly DashboardGroup _group;
		private readonly Dashboard _dashboard;

		public string Title => _group.Title;
        public string Id => _group.Id;
        public bool IsDefault => _group.IsDefault;
		public bool DisplayHeader => !IsDefault && !string.IsNullOrEmpty(Title);

		public int RowCount => (ElementStates.Count / ColumnCount) + ((ElementStates.Count % ColumnCount) > 0 ? 1 : 0);
		public int ColumnCount => _dashboard.ElementsPerRow;

		public ICollection<ElementStateViewModel> ElementStates => _elementStates;


        public DashboardGroupViewModel(Dashboard dashboard, DashboardGroup group)
        {
			_dashboard = dashboard;
			_group = group;
			_elementStates.CollectionChanged += CollectionChanged;
		}

		private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged("RowCount");
		}
	}
}
