﻿using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Swampnet.Dash.Client.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .MinimumLevel.Debug()
             .CreateLogger();

			var bootstrapper = new Boot();
			bootstrapper.Run();
		}
	}
}
