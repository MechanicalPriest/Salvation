using Microsoft.Extensions.Hosting;
using Salvation.Explorer.Modelling;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Salvation.Explorer
{
    class Explorer : IHostedService
    {
        private readonly IHolyPriestExplorer holyPriestExplorer;

        public Explorer(IHolyPriestExplorer holyPriestExplorer)
        {
            this.holyPriestExplorer = holyPriestExplorer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            holyPriestExplorer.GenerateStatWeights();
            holyPriestExplorer.CompareCovenants();
            holyPriestExplorer.TestHolyPriestModel(); // Test stat weights

            return Task.Delay(1);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.Delay(1);
        }
    }
}
