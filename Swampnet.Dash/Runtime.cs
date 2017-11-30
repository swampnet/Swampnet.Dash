using Serilog;
using Swampnet.Dash.Common;
using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Swampnet.Dash
{
    public class Runtime : IRuntime
    {
        private readonly IEnumerable<ITest> _tests;
        private readonly Thread _runtimeThread;

        public Runtime(IEnumerable<ITest> tests)
        {
            _tests = tests;
            _runtimeThread = new Thread(RuntimeThread);
            _runtimeThread.IsBackground = true;
        }


        public void Start()
        {
            _runtimeThread.Start();
        }

        public void Stop()
        {
            // HACK
            _runtimeThread.Abort();
        }

        private void RuntimeThread()
        {
            var runtime = new Dictionary<int, DateTime>();

            while (true)
            {
                try
                {
                    var testDefinitions = new List<TestDefinition>();

                    foreach(var testDefinition in Mock.Tests)
                    {
                        DateTime lastRun;
                        if (!runtime.ContainsKey(testDefinition.Id))
                        {
                            runtime.Add(testDefinition.Id, DateTime.MinValue);
                        }
                        lastRun = runtime[testDefinition.Id];

                        if (lastRun.Add(testDefinition.Heartbeat) < DateTime.UtcNow)
                        {
                            testDefinitions.Add(testDefinition);
                        }
                    }

                    Parallel.ForEach(testDefinitions, async test => 
					{
                        try
                        {
                            var testRunner = _tests.Single(t => t.GetType().Name == test.Type);
                            var rs = await testRunner.RunAsync(test);

                            Log.Information("{test} '{name}' " + rs, 
								testRunner.GetType().Name, 
								test.Name);

							lock (testDefinitions)
							{
								runtime[test.Id] = DateTime.UtcNow;
							}
						}
                        catch (Exception ex)
                        {
                            Log.Error(ex, ex.Message);
                        }
                    });
                }
				catch (ThreadAbortException)
				{
					break;
				}
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
