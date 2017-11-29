using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Service.Services
{
    public interface ITestService
    {
        void Boosh();
    }

    class TestService : ITestService
    {
        public void Boosh()
        {
            Log.Debug("Boosh");
        }
    }
}
