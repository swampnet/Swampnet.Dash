using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface IArgosRepository
    {
        IEnumerable<ArgosDefinition> GetDefinitions();
    }
}
