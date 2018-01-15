using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common
{
    // So, why not just override .Equals() & GetHash() ?
    //
    //  Well - It's not a true equality comparer. We deliberately ignore some properties (eg, Timestamp) and in the future
    //  we might decide to ignore things like case on some or all of the properties.
    //  With that in mind, I thought that .Equals() wasn't a good fit / could have been confusing.
    //
    // @TODO: Would this be better off as extensions over the various entities?
    public static class Compare
    {
        /// <summary>
        /// Compare two ArgosResults
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns>true if lhs and rhs are equal</returns>
        public static bool ArgosResultsAreEqual(ArgosResult lhs, ArgosResult rhs)
        {
            return lhs.ArgosId == rhs.ArgosId
                && IsEqual(lhs.Items, rhs.Items);
        }


        private static bool IsEqual(IEnumerable<ElementState> lhs, IEnumerable<ElementState> rhs)
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
                if (!Compare.DashboardItems(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }


        public static bool DashboardItems(ElementState lhs, ElementState rhs)
        {
            if (lhs.Id != rhs.Id || lhs.Status != rhs.Status)
            {
                return false;
            }

            if(!IsEqual(lhs.Output, rhs.Output))
            {
                return false;
            }

            return true;
        }


        private static bool IsEqual(IEnumerable<Property> lhs, IEnumerable<Property> rhs)
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
                if (!Compare.Properties(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Properties(Property lhs, Property rhs)
        {
            if (lhs.Category != rhs.Category
                || lhs.Name != rhs.Name
                || lhs.Value != rhs.Value)
            {
                return false;
            }

            return true;
        }
    }
}
