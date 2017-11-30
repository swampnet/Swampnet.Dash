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
                Type = "RandomNumberTest",
                Name = "Shake a D6",
                Description = "Shake a D6",
                Heartbeat = TimeSpan.FromSeconds(5),
                Parameters = new List<Property>()
                {
                    new Property("min", "1"),
                    new Property("max", "6"),
                },
                MetaData = new List<Meta>()
                {
                    new Meta("value", "int", "main"),
                    new Meta("D6", "static", "header")
                }
            }
            //,
            //new TestDefinition()
            //{
            //    Id = 1,
            //    Type = "RandomNumberTest",
            //    Name = "Shake a D12",
            //    Description = "Shake a D12",
            //    Heartbeat = TimeSpan.FromSeconds(2),
            //    Parameters = new List<Property>()
            //    {
            //        new Property("min", "1"),
            //        new Property("max", "12"),
            //    },
            //    MetaData = new List<Meta>()
            //    {
            //        new Meta("value", "int", "main"),
            //        new Meta("D12", "static", "header")
            //    }
            //}
        };
    }
}
