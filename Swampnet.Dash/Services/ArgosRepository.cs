using Swampnet.Dash.Common;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Services
{
    class ArgosRepository : IArgosRepository
    {
        // [id => DateTime]
        private readonly Dictionary<string, DateTime> _runtimes = new Dictionary<string, DateTime>();

        public IEnumerable<ArgosDefinition> GetDefinitions()
        {
            return Mock.Argos;
        }


        public IEnumerable<ArgosDefinition> GetPending()
        {
            var definitions = new List<ArgosDefinition>();

            foreach (var definition in GetDefinitions())
            {
                DateTime lastRun;
                if (!_runtimes.ContainsKey(definition.Id))
                {
                    _runtimes.Add(definition.Id, DateTime.MinValue);
                }

                lastRun = _runtimes[definition.Id];

                if (lastRun.Add(definition.Heartbeat) < DateTime.UtcNow)
                {
                    definitions.Add(definition);
                }
            }

            return definitions;
        }

        public void UpdateLastRun(ArgosDefinition definition)
        {
            lock (_runtimes)
            {
                _runtimes[definition.Id] = DateTime.UtcNow;
            }
        }
    }
}
