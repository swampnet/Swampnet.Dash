using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.DAL
{
	class History
	{
		public long Id { get; set; }
		public long RootId { get; set; }
		public DateTime TimestampUtc { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }

		public HistoryRoot Root { get; set; }
	}
}
