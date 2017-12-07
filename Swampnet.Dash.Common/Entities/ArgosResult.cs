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

        //public ArgosResult Clone()
        //{
        //	return new ArgosResult()
        //	{
        //		ArgosId = this.ArgosId,
        //		Items = this.Items.Select(i => new DashboardItem()
        //		{
        //			Id = i.Id,
        //			Status = i.Status,
        //			TimestampUtc = i.TimestampUtc,
        //			Output = i.Output.Select(o => new Property()
        //			{
        //				Category = o.Category,
        //				Name = o.Name,
        //				Value = o.Value
        //			}).ToList()
        //		})
        //	};
        //}

        public override string ToString()
        {
            return $"[{ArgosId}] " + string.Join(", ", Items.Select(i => $"{i.Id}: {i.Status}"));
        }
    }
}
