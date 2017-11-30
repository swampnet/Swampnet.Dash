using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Common
{
    public static class Mock
    {
        public static Dashboard Dash()
        {
            var dash = new Dashboard();
            dash.Id = Guid.NewGuid();
            dash.Name = "Pings";
            dash.Description = "All the pings";
            dash.TestIds = Tests.Where(t => t.Type == "ping").Select(t => t.Id).ToList();
            return dash;
        }

        public static IEnumerable<TestDefinition> Tests => _tests;


        private static readonly TestDefinition[] _tests = new[]
        {
            new TestDefinition()
            {
                Id = 1,
                Type = "ping",
                Name = "Ping Google",
                Description = "Ping the googs",
                Heartbeat = TimeSpan.FromSeconds(2),
                Parameters = new List<Property>()
                {
                    new Property("endpoint", "http://www.google.com")
                },
                MetaData = new List<Meta>()
                {
                    new Meta("value", "int", "main"),
                    new Meta("Google", "static", "header"),
                    new Meta("Ping", "static", "footer"),
                }
            },
            new TestDefinition()
            {
                Id = 2,
                Type = "ping",
                Name = "Ping BBC",
                Description = "Ping the beeb",
                Heartbeat = TimeSpan.FromSeconds(2),
                Parameters = new List<Property>()
                {
                    new Property("endpoint", "http://www.bbc.co.uk")
                },
                MetaData = new List<Meta>()
                {
                    new Meta("value", "int", "main"),
                    new Meta("The Beeb", "static", "header"),
                    new Meta("Ping", "static", "footer"),
                }
            }
        };
    }
}
