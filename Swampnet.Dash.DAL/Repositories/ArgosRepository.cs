using Swampnet.Dash.Common;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.DAL
{
    class ArgosRepository : IArgosRepository
    {
        public IEnumerable<ArgosDefinition> GetDefinitions()
        {
            return Mock.Argos;
        }
    }
}
