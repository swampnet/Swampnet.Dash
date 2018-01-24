using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swampnet.Dash.Common.Entities;
using Serilog;
using Swampnet.Dash.Common;
using Autofac;

namespace Swampnet.Dash.Services
{
	internal class ArgosRunner : IArgosRunner
	{
        private readonly IEnumerable<IArgos> _argos;
        private readonly IArgosRepository _argosRepository;
        private readonly Dictionary<string, ArgosResult> _state = new Dictionary<string, ArgosResult>();
		private readonly IRuleProcessor _ruleProcessor;
		private readonly IStateProcessor _stateProcessor;
		private readonly ILifetimeScope _scope;

		public ArgosRunner(IArgosRepository argosRepo, IEnumerable<IArgos> argos, IRuleProcessor ruleProcessor, IStateProcessor stateProcessor, ILifetimeScope scope)
        {
            _argos = argos;
            _argosRepository = argosRepo;
			_ruleProcessor = ruleProcessor;
			_stateProcessor = stateProcessor;
			_scope = scope;
		}

		/// <summary>
		/// Return results of Argos instances have been updated
		/// </summary>
		/// <returns></returns>
        public async Task<IEnumerable<ArgosResult>> RunAsync()
        {
			var items = new List<ArgosResult>();

			foreach (var instance in Instances.Where(a => a.IsDue))
			{
				try
				{
					//Log.Debug("Running {type} - {name}", instance.GetType().Name, instance.Id);

					// Run instance
					var result = await instance.RunAsync();

					await _stateProcessor.ProcessAsync(instance.Definition, result.Items);
					await _ruleProcessor.ProcessAsync(instance.Definition, result.Items);

					// Check if state has changed since last time
					ArgosResult previous = null;
					if (_state.ContainsKey(instance.Id))
					{
						previous = _state[instance.Id];
					}

					if (!Compare.IsEqual(result, previous))
					{
						items.Add(result);

						// #hack: dump result
						Log.Debug(result.ToString());
					}

					_state[instance.Id] = result.Copy();
				}
				catch (Exception ex)
				{
					Log.Error(ex, ex.Message);
				}
			}

			#region original
			//// Get any argos definitions who's heartbeat is due
			//foreach (var definition in GetDue())
			//         {
			//	try
			//	{
			//                 ArgosResult lastRun;
			//                 if (_state.ContainsKey(definition.Id))
			//                 {
			//                     lastRun = _state[definition.Id];
			//                 }
			//                 else
			//                 {
			//                     lastRun = new ArgosResult();
			//                     _state.Add(definition.Id, lastRun);
			//                 }

			//                 var argos = _argos.Single(t => t.GetType().Name == definition.Type);

			//		//Validate(testdefinition, test.Meta);
			//		Log.Debug("Running {type} - {name}", argos.GetType().Name, definition.Id);

			//		var rs = await argos.RunAsync(definition);

			//		foreach(var item in rs.Items)
			//		{
			//			await _stateProcessor.ProcessAsync(definition, item);
			//			await _ruleProcessor.ProcessAsync(definition, item);
			//		}

			//                 if (!Compare.ArgosResultsAreEqual(rs, lastRun))
			//                 {
			//                     //Log.Information("{argos} '{id}' Has changed: " + rs,
			//                     //    argos.GetType().Name,
			//                     //    definition.Id);

			//                     items.Add(rs);
			//                 }

			//                 _state[definition.Id] = rs;
			//             }
			//             catch (Exception ex)
			//             {
			//                 Log.Error(ex, ex.Message);
			//             }
			//         }
			#endregion

			return items;
        }

		private IEnumerable<IArgos> _instances;

		public IEnumerable<IArgos> Instances
		{
			get
			{
				if(_instances == null)
				{
					var instances = new List<IArgos>();
					foreach(var definition in _argosRepository.GetDefinitions())
					{
						try
						{
							// @todo: Maybe pull this out into a TestFactory service, then you can keep your smelly service locator bullshit out of here!
							var x = Type.GetType(definition.Type);
							if (x != null)
							{
								var instance = _scope.Resolve(x) as IArgos;
								if (instance != null)
								{
									instance.Configure(definition);
									instances.Add(instance);
								}
							}
						}
						catch (Exception ex)
						{
							Log.Error(ex, "Failed to create argos '{argos}'", definition.Id);
						}
					}
					_instances = instances;
				}
				return _instances;
			}
		}


        // @TODO: Does this really belong in here?
        public Task<ArgosResult> GetState(string id)
        {
            ArgosResult rs = _state.ContainsKey(id)
                ? _state[id]
                : null;

            return Task.FromResult(rs);
        }
    }
}
