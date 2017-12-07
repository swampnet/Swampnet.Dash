using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;
using Serilog;

namespace Swampnet.Dash.Services
{
	internal class ArgosRunner : IArgosRunner
	{
        private readonly IEnumerable<IArgos> _argos;
        private readonly IArgosRepository _argosRepo;

        // [argos-definition-id] -> argos
        private readonly Dictionary<string, ArgosResult> _state = new Dictionary<string, ArgosResult>();


        public ArgosRunner(IArgosRepository argosRepo, IEnumerable<IArgos> argos)
        {
            _argos = argos;
            _argosRepo = argosRepo;
        }


        public async Task<IEnumerable<ArgosResult>> RunAsync()
        {
            var items = new List<ArgosResult>();

            // Get any argos definitions who's heartbeat is due
            foreach (var definition in GetDue())
            {
				try
				{
                    ArgosResult lastRun;
                    if (_state.ContainsKey(definition.Id))
                    {
                        lastRun = _state[definition.Id];
                    }
                    else
                    {
                        lastRun = new ArgosResult();
                        _state.Add(definition.Id, lastRun);
                    }

                    var argos = _argos.Single(t => t.GetType().Name == definition.Type);

                    //Validate(testdefinition, test.Meta);

                    var rs = await argos.RunAsync(definition);

                    // Only add to our results if something has changed
                    // @TODO: We *might* want to only update if something we have mapped in meta data has changed...
                    //          eg: We might be returning a 'timestamp' property that *will* change each heartbeat, but we don't have it mapped to anything
                    //              so we don't really care if it changes or not. (rememmber, this is telling the client something has updated. If the client 
                    //              isn't interested in any of the stuff that's changed then we don't need to broadcast it.)
                    //              - That screws our equality comparer implementation doesn't it...
                    if (!rs.Equals(lastRun))
                    {
                        Log.Information("{argos} '{id}' Has changed: " + rs,
                            argos.GetType().Name,
                            definition.Id);

                        items.Add(rs);
                    }

                    _state[definition.Id] = rs;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }

            return items;
        }


        public IEnumerable<ArgosDefinition> GetDue()
        {
            var definitions = new List<ArgosDefinition>();

            foreach (var definition in _argosRepo.GetDefinitions())
            {
                // Never been run
                if (!_state.ContainsKey(definition.Id))
                {
                    definitions.Add(definition);
                }
                else
                {
                    var lastResult = _state[definition.Id];
                    if (lastResult.TimestampUtc.Add(definition.Heartbeat) < DateTime.UtcNow)
                    {
                        definitions.Add(definition);
                    }
                }
            }

            return definitions;
        }



        public Task<ArgosResult> GetState(string id)
        {
            ArgosResult rs = _state.ContainsKey(id)
                ? _state[id]
                : null;

            return Task.FromResult(rs);
        }
    }
}
