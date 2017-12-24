using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Prism.Mvvm;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swampnet.Dash.Client.Wpf.ViewModels
{
	class DashboardItemViewModel : BindableBase
	{
		private Element _element;
		private readonly Dashboard _dashboard;
		private readonly ItemDefinition _itemDefinition;

		static DashboardItemViewModel()
		{
			// Map x axis to seconds
			var mapper = Mappers.Xy<Varient>()
				.X(m => (DateTime.UtcNow - m.Timestamp).TotalSeconds)
				.Y(m => m.Value);

			Charting.For<Varient>(mapper);
		}

		public DashboardItemViewModel(Dashboard dashboard, ItemDefinition itemDefinition, object id)
		{
			_dashboard = dashboard;
			_itemDefinition = itemDefinition;

            Id = id.ToString();

			Series = new SeriesCollection
			{
				new LineSeries
				{
					AreaLimit = -10,
					Values = new ChartValues<Varient>()
				}
			};

			LoadHistory(); // @todo: .ContinueWith -> Start updating
		}


		public void Update(Element element)
		{
			if (_loadingHistory)
			{
				return; // HACK
			}

			_element = element;

			AddHistory(new Varient(element.TimestampUtc, element.Output.DoubleValue("value", 0.0)));

			RaisePropertyChanged("");
		}


		private void AddHistory(Varient data)
		{
			Series[0].Values.Add(data);

			// @todo: this, properly!
			while (Series[0].Values.Count > 100)
			{
				Series[0].Values.RemoveAt(0);
			}
		}

		public SeriesCollection Series { get; set; }
		public bool DisplayGraph => _itemDefinition.Plot != null;
		public string Id { get; private set; }
        public DateTime? Timestamp => _element?.TimestampUtc;
		public Status Status => _element == null ? Status.Unknown : _element.Status;
		public string Order
		{
			get
			{
				var value = GetValue("order");
				if (string.IsNullOrEmpty(value))
				{
					value = _element?.Order; // @todo: Possibly the other way round? Use dashItem.Order first, falling back to mapping? Dunno.
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


		#region Graphing stuff

		public int AxisYMinValue => DisplayGraph
			? _itemDefinition.Plot.MinY
			: 0;

		public int AxisYMaxValue => DisplayGraph
			? _itemDefinition.Plot.MaxY
			: 0;

		public int HistorySeconds => DisplayGraph
			? (int)_itemDefinition.Plot.History.TotalSeconds
			: 0;

		#endregion

		//@TODO: Need unit tests all over this!
		private string GetValue(string region)
		{
			string value = null;

			var meta = GetMeta().SingleOrDefault(m => m.Region.EqualsNoCase(region));

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
							value = (_element == null ? Status.Unknown : _element.Status).ToString();
							break;

						case "timestamputc":
							value = _element?.TimestampUtc.ToLongTimeString(); // .Format()
							break;

						case "id":
							value = _element?.Id;
							break;

						// Check dash item properties
						default:
							value = _element?.Output.StringValue(meta.Name);
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


		private IEnumerable<Meta> GetMeta()
		{
			if(_itemDefinition.MetaData != null && _itemDefinition.MetaData.Any())
			{
				return _itemDefinition.MetaData;
			}
			else if(_dashboard.DefaultMetaData != null && _dashboard.DefaultMetaData.Any())
			{
				return _dashboard.DefaultMetaData;
			}
			else
			{
				return Enumerable.Empty<Meta>();
			}
		}

		private bool _loadingHistory = false;

		private async Task LoadHistory()
		{
			if (_itemDefinition.Plot != null)
			{
				_loadingHistory = true;

				var data = await Api.GetHistory(_itemDefinition.Id, _itemDefinition.Plot.PropertyName, _itemDefinition.Plot.History);
				foreach (var d in data.OrderBy(x => x.Timestamp))
				{
					AddHistory(d);
				}

				_loadingHistory = false;
			}
		}
	}
}
