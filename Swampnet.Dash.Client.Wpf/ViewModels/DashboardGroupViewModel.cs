﻿using Prism.Mvvm;
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

        public string Title => _group.Title;
        public string Id => _group.Id;
        public bool IsDefault => _group.IsDefault;
		public bool DisplayHeader => !IsDefault && !string.IsNullOrEmpty(Title);

		public ICollection<ElementStateViewModel> Items => _items;


        public DashboardGroupViewModel(DashboardGroup group)
        {
            _group = group;
        }
    }
}
