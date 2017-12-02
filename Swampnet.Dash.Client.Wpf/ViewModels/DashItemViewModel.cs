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
			_dashItem = dashItem;
			RaisePropertyChanged("");
		}

		public string Id => _dashItem.Id;

		public DateTime Timestamp => _dashItem.TimestampUtc;

		public string Main
		{
			get
			{
				// @TODO: Use meta data to figure this out
				return _dashItem.Properties.StringValue("value");
			}
		}

		public string Header
		{
			get
			{
				// @TODO: Use meta data to figure this out
				return _dashItem.Id;
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
			get { return null; }
		}

		public string FooterRight
		{
			get { return null; }
		}
	}
}
