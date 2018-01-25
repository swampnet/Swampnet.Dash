using Swampnet.Dash.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.Common.Interfaces
{
    public interface ITest
    {
        void Configure(Element definition);

        Task<ElementState> ExecuteAsync();

		TestMeta Meta { get; }

        string Id { get; }

        bool IsDue { get; }

		Element Definition { get; }

		ElementState State { get; }
	}
}
