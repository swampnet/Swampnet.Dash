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

        // argos-id -> argos
        private readonly Dictionary<string, ArgosResult> _lastResults = new Dictionary<string, ArgosResult>();

        public ArgosRunner(IArgosRepository argosRepo, IEnumerable<IArgos> argos)
        {
            _argos = argos;
            _argosRepo = argosRepo;
        }

        public async Task<IEnumerable<ArgosResult>> RunAsync()
        {
            var items = new List<ArgosResult>();

            foreach (var definition in _argosRepo.GetPending())
            {
                try
                {
                    ArgosResult lastRun;
                    if (_lastResults.ContainsKey(definition.Id))
                    {
                        lastRun = _lastResults[definition.Id];
                    }
                    else
                    {
                        lastRun = new ArgosResult();
                        _lastResults.Add(definition.Id, lastRun);
                    }

                    var argos = _argos.Single(t => t.GetType().Name == definition.Type);

                    //Validate(testdefinition, test.Meta);

                    var rs = await argos.RunAsync(definition);

                    // Only add to our results if something has changed
                    bool hacky_bool = Changed(rs, lastRun);
                    Log.Debug("Changed: {changed}", hacky_bool);
                    if (hacky_bool)
                    {
                        items.Add(rs);

                        Log.Information("{argos} '{id}' " + rs.Items,
                            argos.GetType().Name,
                            definition.Id);
                    }

                    _argosRepo.UpdateLastRun(definition);
                    _lastResults[definition.Id] = rs;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }

            return items;
        }



        public Task<ArgosResult> GetState(string id)
        {
            ArgosResult rs = _lastResults.ContainsKey(id)
                ? _lastResults[id]
                : null;

            return Task.FromResult(rs);
        }


        // Need some way of exposing this to unit tests (i think it's buggy, i think if we remove something from lhs we won't pick it up)
        private bool Changed(ArgosResult lhs, ArgosResult rhs)
        {
            foreach(var l in lhs.Items)
            {
                var r = rhs.Items?.SingleOrDefault(x => x.Id == l.Id);
                if(r == null)
                {
                    // Removed / Added an item
                    return true;
                }

                if(l.Status != r.Status)
                {
                    // Status has changed
                    return true;
                }

                // Check output
                foreach (var l_o in l.Output)
                {
                    var r_o = r.Output.SingleOrDefault(x => x.Name == l_o.Name);
                    if(r_o == null)
                    {
                        // Removed / Added a property
                        return true;
                    }
                    if(r_o.Value != l_o.Value)
                    {
                        // A property has changed
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
