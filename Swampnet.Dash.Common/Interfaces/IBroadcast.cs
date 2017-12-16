using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IBroadcast
    {
        void Update(string group, IEnumerable<Element> dashItems);
        void Refresh(string group, IEnumerable<Element> dashItems);
    }
}
