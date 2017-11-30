using Swampnet.Dash.Common.Entities;
using Swampnet.Dash.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Swampnet.Dash.Tests
{
    class RandomNumberTest : ITest
    {
        private readonly Random _rnd = new Random();

        public void Update()
        {
        }
    }
}
