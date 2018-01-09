using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public enum Status
    {
        Unknown = 0,
        Ok = 1,
        Warn = 2,
        Alert = 3,
        Error = 4
    }

	public static class Constants
	{
		public const string MANDATORY_CATEGORY = "mandatory";
		public const string OPTIONAL_CATEGORY = "optional";
	}
}
