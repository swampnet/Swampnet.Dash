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
        int Id { get; }
    }

    class TestService : ITestService
    {
        private static int _id;

        public int Id => _id;


        public TestService()
        {
            _id = ++_id;
        }

        public void Boosh()
        {
            Log.Debug("Boosh");
        }
    }
}
