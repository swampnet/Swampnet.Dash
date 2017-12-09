using Swampnet.Dash.Client.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Swampnet.Dash.Client.Wpf.Views
{
	public class DashboardTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var element = container as FrameworkElement;

			if (element != null && item != null)
			{
				var dashboard = item as DashboardViewModel;
				if(dashboard != null && !string.IsNullOrEmpty(dashboard.Template))
				{
					return element.FindResource(dashboard.Template) as DataTemplate;
				}
			}

			return null;
		}
	}
}
