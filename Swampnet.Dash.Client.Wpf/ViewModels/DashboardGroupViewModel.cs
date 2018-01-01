using Prism.Mvvm;
using Swampnet.Dash.Common.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Swampnet.Dash.Client.Wpf.ViewModels
{
    /// <summary>
    /// Group view model
    /// </summary>
    class DashboardGroupViewModel : BindableBase
    {
        private readonly ObservableCollection<ElementStateViewModel> _items = new ObservableCollection<ElementStateViewModel>();
        private readonly DashboardGroup _group;
		private readonly Dashboard _dashboard;

		public string Title => _group.Title;
        public string Id => _group.Id;
        public bool IsDefault => _group.IsDefault;
		public bool DisplayHeader => !IsDefault && !string.IsNullOrEmpty(Title);

		public int RowCount => (ElementStates.Count / ColumnCount) + 1;
		public int ColumnCount => _dashboard.ElementsPerRow;

		public ICollection<ElementStateViewModel> ElementStates => _items;


        public DashboardGroupViewModel(Dashboard dashboard, DashboardGroup group)
        {
			_dashboard = dashboard;
			_group = group;
			_items.CollectionChanged += _items_CollectionChanged;
		}

		private void _items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged("RowCount");
		}
	}
}
