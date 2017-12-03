using Prism.Mvvm;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Client.Wpf.ViewModels
{
	class DashItemViewModel : BindableBase
	{
		private readonly DashItemMeta _meta;
		private DashItem _dashItem;

		public DashItemViewModel(DashItemMeta meta)
		{
			_meta = meta;
		}

		public void Update(DashItem dashItem)
		{
			if(dashItem.Id == _meta.Id)
			{
				_dashItem = dashItem;
				RaisePropertyChanged("");
			}
		}

		public string Id => _meta.Id;
		public DateTime? Timestamp => _dashItem?.TimestampUtc;
		public string State => _dashItem?.State;

		public string Main
		{
			get
			{
				// @TODO: Use meta data to figure this out
				return _dashItem?.Properties.StringValue("value");
			}
		}

		public string Header
		{
			get
			{
				// @TODO: Use meta data to figure this out
				return _meta.Name;
			}
		}

		public string HeaderLeft
		{
			get { return null; }
		}

		public string HeaderRight
		{
			get { return null; }
		}

		public string Footer
		{
			get { return null; }
		}

		public string FooterLeft
		{
			get
			{
				// @TODO: Use meta data to figure this out
				return _dashItem?.State;
			}
		}

		public string FooterRight
		{
			get
			{
				// @TODO: Use meta data to figure this out
				return _dashItem?.TimestampUtc.ToLongTimeString();
			}
		}
	}
}
