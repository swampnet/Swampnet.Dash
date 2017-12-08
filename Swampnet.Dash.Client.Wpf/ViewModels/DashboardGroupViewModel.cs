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
        private readonly ObservableCollection<DashboardItemViewModel> _items = new ObservableCollection<DashboardItemViewModel>();
        private readonly DashboardGroup _group;

        public string Title => _group.Title;
        public string Id => _group.Id;
        public bool IsDefault => _group.IsDefault;

        public ICollection<DashboardItemViewModel> Items => _items;


        public DashboardGroupViewModel(DashboardGroup group)
        {
            _group = group;
        }
    }
}
