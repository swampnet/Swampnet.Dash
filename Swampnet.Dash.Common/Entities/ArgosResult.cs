using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class ArgosResult
    {
        public string ArgosId { get; set; }
        public IEnumerable<DashboardItem> Items { get; set; }

		public override bool Equals(object obj)
		{
			var source = obj as ArgosResult;
			if(source == null)
			{
				return false;
			}
			if(source.ArgosId != this.ArgosId)
			{
				return false;
			}

			return Items.OrderBy(x => x.Id).SequenceEqual(source.Items.OrderBy(x => x.Id));
		}

		// @TODO: This, properly.
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
