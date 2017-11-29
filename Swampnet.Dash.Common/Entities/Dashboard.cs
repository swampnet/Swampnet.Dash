using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class Dashboard
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // @TODO: ItemSource?
        // Sometimes the items can be dynamic (ie, ArgosDash)
    }
}
