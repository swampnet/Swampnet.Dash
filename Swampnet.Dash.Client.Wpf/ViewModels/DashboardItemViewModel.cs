﻿using Prism.Mvvm;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swampnet.Dash.Client.Wpf.ViewModels
{
	class DashboardItemViewModel : BindableBase
	{
		private readonly IEnumerable<Meta> _meta;
		private DashboardItem _dashItem;

		public DashboardItemViewModel(object id, IEnumerable<Meta> meta)
		{
			_meta = meta;
            Id = id.ToString();
		}

		public void Update(DashboardItem dashItem)
		{
			_dashItem = dashItem;
			RaisePropertyChanged("");
		}

        public string Id { get; private set; }
        public DateTime? Timestamp => _dashItem?.TimestampUtc;
		public string Status => _dashItem?.Status;
        public string Order => _dashItem?.Order;

		public string Group
		{
			get { return GetValue("group"); }
		}


		public string Main
		{
			get { return GetValue("main"); }
		}

		public string Header
		{
			get { return GetValue("header"); }
		}

		public string HeaderLeft
		{
			get { return GetValue("header-left"); }
		}

		public string HeaderRight
		{
			get { return GetValue("header-right"); }
		}

		public string Footer
		{
			get { return GetValue("footer"); }
		}

		public string FooterLeft
		{
			get { return GetValue("footer-left"); }
		}

		public string FooterRight
		{
			get { return GetValue("footer-right"); }
		}

		//@TODO: Need unit tests all over this!
		private string GetValue(string region)
		{
			string value = null;

			var meta = _meta.SingleOrDefault(m => m.Region.EqualsNoCase(region));
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
						case "status":
							value = _dashItem?.Status;
							break;

						case "timestamputc":
							value = _dashItem?.TimestampUtc.ToLongTimeString(); // .Format()
							break;

						case "id":
							value = _dashItem?.Id;
							break;

						// Check dash item properties
						default:
							value = _dashItem?.Output.StringValue(meta.Name);
							break;
					}
				}

				value = Format(meta.Type, meta.Format, value);
			}

			return value;
		}


		private string Format(string type, string format, string value)
		{
			if (!string.IsNullOrEmpty(format) && !string.IsNullOrEmpty(value))
			{
				switch (type.ToLowerInvariant())
				{
					case "decimal":
					case "double":
					case "float":
						value = string.Format("{0:" + format.Trim() + "}", Convert.ToDouble(value));
						break;

					case "datetime":
						value = string.Format("{0:" + format.Trim() + "}", Convert.ToDateTime(value));
						break;
				}
			}

			return value;
		}
	}
}
