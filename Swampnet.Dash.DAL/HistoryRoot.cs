using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.DAL
{
	class HistoryRoot
	{
		public HistoryRoot()
		{
			History = new List<History>();
		}

		public long Id { get; set; }
		public string Name { get; set; }

		public ICollection<History> History { get; set; }
	}
}
