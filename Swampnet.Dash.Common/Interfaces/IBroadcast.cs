using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IBroadcast
    {
        void Update(string group, IEnumerable<ElementState> dashItems);
        void Refresh(string group, IEnumerable<ElementState> dashItems);
    }
}
