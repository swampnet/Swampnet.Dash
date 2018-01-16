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
	class ElementStateViewModel : BindableBase
	{
		private ElementState _element;
		private readonly Dashboard _dashboard;
		private readonly ElementDefinition _itemDefinition;

		static ElementStateViewModel()
		{
			// Map x axis to seconds
			var mapper = Mappers.Xy<Variant>()
				.X(m => (DateTime.UtcNow - m.TimestampUtc).TotalSeconds)
				.Y(m => m.Value);

			Charting.For<Variant>(mapper);
		}

		public ElementStateViewModel(Dashboard dashboard, ElementDefinition itemDefinition, object id)
		{
			_dashboard = dashboard;
			_itemDefinition = itemDefinition;

            Id = id.ToString();

			Series = new SeriesCollection
			{
				new LineSeries
				{
					AreaLimit = -10,
					Values = new ChartValues<Variant>()
				}
			};

			// We don't want to start updating until we've loaded the history (or at least manage it somehow)
			LoadHistory(); // @todo: .ContinueWith -> Start updating
		}

		private List<Variant> _buffer = new List<Variant>();

		public void Update(ElementState element)
		{
			_element = element;
			
			var variant = new Variant(element.TimestampUtc, element.Output.DoubleValue(_itemDefinition.Plot.Output, 0.0));

			// If we're currently loading the history, ignore the update for now (but keep track of the incoming data so we can merge it later)
			if (_loadingHistory)
			{
				_buffer.Add(variant);
				return;
			}
			
			AddVariant(variant);

			RaisePropertyChanged("");
		}


		private void AddVariant(Variant data)
		{
			if(_itemDefinition.Plot != null)
			{
				// Add value if it doesn't already exist
				if (!Series[0].Values.Cast<Variant>().Any(h => h.TimestampUtc == data.TimestampUtc))
				{
					Series[0].Values.Add(data);
				}

				// truncate everything older than this date
				var dt = DateTime.UtcNow.Subtract(_itemDefinition.Plot.History);

				// @todo: this, properly!
				while (Series[0].Values.Count > 100)
				{
					Series[0].Values.RemoveAt(0);
				}
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

		#region Regions
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
		#endregion


		#region Graphing stuff

		public double AxisYMinValue => DisplayGraph
			? _itemDefinition.Plot.MinY
			: 0;

		public double AxisYMaxValue => DisplayGraph
			? _itemDefinition.Plot.MaxY
			: 0;

		public int HistorySeconds => DisplayGraph
			? (int)_itemDefinition.Plot.History.TotalSeconds
			: 0;

		#endregion

		//@TODO: Need unit tests all over this!
		// Get value for a particular region
		private string GetValue(string region)
		{
			string value = null;

			var meta = GetMeta().SingleOrDefault(m => m.Region.EqualsNoCase(region));

			if(meta != null)
			{
				// Check for constant value
				if (!string.IsNullOrEmpty(meta.Constant))
				{
					value = meta.Constant;
				}
				// Else it's a property lookup
				else
				{
					switch (meta.Property.ToLowerInvariant())
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
							value = _element?.Output.StringValue(meta.Property);
							break;
					}

					value = Format(meta.Type, meta.Format, value);
				}
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


		private IEnumerable<Map> GetMeta()
		{
			if(_itemDefinition.Mapping != null && _itemDefinition.Mapping.Any())
			{
				return _itemDefinition.Mapping;
			}
			else if(_dashboard.Mapping != null && _dashboard.Mapping.Any())
			{
				return _dashboard.Mapping;
			}
			else
			{
				return Enumerable.Empty<Map>();
			}
		}

		private bool _loadingHistory = false;

		private async Task LoadHistory()
		{
			if (_itemDefinition.Plot != null)
			{
				_loadingHistory = true;

				var data = await Api.GetHistory(_itemDefinition.Id, _itemDefinition.Plot.Output, _itemDefinition.Plot.History);
				foreach (var d in data.Concat(_buffer).OrderBy(x => x.TimestampUtc))
				{
					AddVariant(d);
				}

				_loadingHistory = false;
			}
		}
	}
}
