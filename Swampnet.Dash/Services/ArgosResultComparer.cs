using Serilog;
using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Services
{
    class ArgosResultComparer
    {
        public bool IsEqual(ArgosResult lhs, ArgosResult rhs)
        {
            return lhs.ArgosId == rhs.ArgosId
                && IsEqual(lhs.Items, rhs.Items);
        }

        private bool IsEqual(IEnumerable<DashboardItem> lhs, IEnumerable<DashboardItem> rhs)
        {
            if((lhs != null && rhs == null) || (lhs == null && rhs != null))
            {
                return false;
            }

            if (lhs.Count() != rhs.Count())
            {
                return false;
            }

            var x = lhs.OrderBy(d => d.Id).ToArray();
            var y = rhs.OrderBy(d => d.Id).ToArray();
            for (int i = 0; i < x.Length; i++)
            {
                if (!IsEqual(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsEqual(DashboardItem lhs, DashboardItem rhs)
        {
            if (lhs.Id != rhs.Id || lhs.Status != rhs.Status)
            {
                return false;
            }

            return true;
        }


        private bool IsEqual(IEnumerable<Property> lhs, IEnumerable<Property> rhs)
        {
            if ((lhs != null && rhs == null) || (lhs == null && rhs != null))
            {
                return false;
            }

            if (lhs.Count() != rhs.Count())
            {
                return false;
            }

            var x = lhs.OrderBy(d => d.Category).ThenBy(d => d.Name).ToArray();
            var y = rhs.OrderBy(d => d.Category).ThenBy(d => d.Name).ToArray();
            for (int i = 0; i < x.Length; i++)
            {
                if (!IsEqual(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsEqual(Property lhs, Property rhs)
        {
            if (lhs.Category != rhs.Category
                || lhs.Name != rhs.Name
                || lhs.Value != rhs.Value)
            {
                Log.Debug($"IsEqual-False (Property Category/Name/Value)");
                return false;
            }

            return true;
        }
    }
}
