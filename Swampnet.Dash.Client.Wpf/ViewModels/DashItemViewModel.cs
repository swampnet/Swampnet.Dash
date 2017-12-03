using Prism.Mvvm;
using Swampnet.Dash.Common.Entities;
using System;
using System.Linq;

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
			if (dashItem.Id == _meta.Id)
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
			get { return ResolveValue("main"); }
		}

		public string Header
		{
			get { return ResolveValue("header"); }
		}

		public string HeaderLeft
		{
			get { return ResolveValue("header-left"); }
		}

		public string HeaderRight
		{
			get { return ResolveValue("header-right"); }
		}

		public string Footer
		{
			get { return ResolveValue("footer"); }
		}

		public string FooterLeft
		{
			get { return ResolveValue("footer-left"); }
		}

		public string FooterRight
		{
			get { return ResolveValue("footer-right"); }
		}

		//@TODO: Need unit tests all over this!
		private string ResolveValue(string region)
		{
			string value = null;

			var meta = _meta?.Meta.SingleOrDefault(m => m.Region.EqualsNoCase(region));
			if(meta != null)
			{
				// Check static values
				if (meta.Type.EqualsNoCase("static"))
				{
					value = meta.Name;
				}
				else
				{
					switch (meta.Name.ToLowerInvariant())
					{
						// Check some known values
						case "state":
							value = _dashItem?.State;
							break;

						case "timestamputc":
							value = _dashItem?.TimestampUtc.ToLongTimeString(); // .Format()
							break;

						case "id":
							value = _dashItem?.Id;
							break;

						// Check dash item properties
						default:
							value = _dashItem?.Properties.StringValue(meta.Name);
							break;
					}
				}
			}

			return value;
		}
	}
}
