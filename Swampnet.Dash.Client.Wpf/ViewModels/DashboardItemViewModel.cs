﻿using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Prism.Mvvm;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swampnet.Dash.Client.Wpf.ViewModels
{
	class DashboardItemViewModel : BindableBase
	{
		private readonly IEnumerable<Meta> _meta;
		private Element _dashItem;

		static DashboardItemViewModel()
		{
			// Map x axis to seconds
			var mapper = Mappers.Xy<Varient>()
				.X(m => (DateTime.UtcNow - m.Timestamp).TotalSeconds)
				.Y(m => m.Value);

			Charting.For<Varient>(mapper);
		}

		public DashboardItemViewModel(object id, IEnumerable<Meta> meta)
		{
			_meta = meta;
            Id = id.ToString();

			Series = new SeriesCollection
			{
				new LineSeries
				{
					AreaLimit = -10,
					Values = new ChartValues<Varient>()
				}
			};
		}

		public void Update(Element dashItem)
		{
			_dashItem = dashItem;

			Series[0].Values.Add(new Varient(dashItem.TimestampUtc, dashItem.Output.DoubleValue("value", 0.0)));

			while (Series[0].Values.Count > 100)
			{
				Series[0].Values.RemoveAt(0);
			}


			RaisePropertyChanged("");
		}

		public SeriesCollection Series { get; set; }

		public string Id { get; private set; }
        public DateTime? Timestamp => _dashItem?.TimestampUtc;
		public Status Status => _dashItem == null ? Status.Unknown : _dashItem.Status;
		public string Order
		{
			get
			{
				var value = GetValue("order");
				if (string.IsNullOrEmpty(value))
				{
					value = _dashItem?.Order; // @todo: Possibly the other way round? Use dashItem.Order first, falling back to mapping? Dunno.
				}
				return value;
			}
		}

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
							value = (_dashItem == null ? Status.Unknown : _dashItem.Status).ToString();
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
