using Microsoft.Extensions.Hosting;
using Salvation.Explorer.Modelling;
using System.Threading;
using System.Threading.Tasks;

namespace Salvation.Explorer
{
    class Explorer : IHostedService
    {
        private readonly IHolyPriestExplorer _holyPriestExplorer;

        public Explorer(IHolyPriestExplorer holyPriestExplorer)
        {
            _holyPriestExplorer = holyPriestExplorer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _holyPriestExplorer.GenerateStatWeights();
            _holyPriestExplorer.CompareCovenants();
            _holyPriestExplorer.TestHolyPriestModel(); // Test stat weights

            return Task.Delay(1);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.Delay(1);
        }
    }
}
