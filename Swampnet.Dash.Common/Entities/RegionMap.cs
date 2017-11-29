using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    /// <summary>
    /// Metadata - Map Item properties to regions
    /// - Formatting options (ie, decimal places)
    /// </summary>
    public class RegionMap
    {
        public RegionMap()
        {
            Map = new List<PropertyToRegionMap>();
        }

        public Guid Id { get; set; }
        
        /// <summary>
        /// Reference to the item this is for.
        /// Can't we map this to some kind of 'type' rather than an ItemId (Argos: All the items use the same map, all have different, transient, Id's)
        /// </summary>
        public Guid ItemId { get; set; }


        public ICollection<PropertyToRegionMap> Map { get; private set; }
    }


    /// <summary>
    /// Maps a given property to a region
    /// </summary>
    public class PropertyToRegionMap
    {
        public string PropertyName { get; set; }
        public string RegionName { get; set; }

        /// <summary>
        /// Standard c# format string.
        /// This isn't going to work unless we know the type...
        /// </summary>
        public string Format { get; set; }
    }
}
