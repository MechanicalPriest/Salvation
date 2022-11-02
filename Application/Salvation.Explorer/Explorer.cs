using Microsoft.Extensions.Hosting;
using Salvation.Explorer.Modelling;
using Salvation.Utility.SpellDataUpdate;
using Salvation.Utility.TalentStructureUpdate;
using System.Threading;
using System.Threading.Tasks;

namespace Salvation.Explorer
{
    class Explorer : IHostedService
    {
        private readonly string[] _args;
        private readonly IHolyPriestExplorer _holyPriestExplorer;
        private readonly ISpellDataUpdateService _spellDataUpdateService;
        private readonly ITalentStructureUpdateService _talentStructureUpdateService;

        public Explorer(string[] args,
            IHolyPriestExplorer holyPriestExplorer,
            ISpellDataUpdateService spellDataUpdateService,
            ITalentStructureUpdateService talentStructureUpdateService)
        {
            _args = args;
            _holyPriestExplorer = holyPriestExplorer;
            _spellDataUpdateService = spellDataUpdateService;
            _talentStructureUpdateService = talentStructureUpdateService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var arg in _args)
            {
                if (string.IsNullOrEmpty(arg) || arg.Length < 2 || arg[0] != '-')
                    continue;

                switch (arg.Substring(1).ToLower())
                {
                    case "updatespelldata":
                        await _spellDataUpdateService.UpdateSpellData();
                        break;
                    case "generatestatweights":
                        await _holyPriestExplorer.GenerateStatWeights();
                        break;
                    case "testholypriest":
                        await _holyPriestExplorer.TestHolyPriestModelAsync(); // Test stat weights
                        break;
                    case "updatetalentdata":
                        await _talentStructureUpdateService.UpdateTalentStructure(); // Update talent data from raidbots
                        break;
                    default:
                        break;
                }
            }
            System.Console.WriteLine("Done running all tasks. Ctrl + C to exit");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1);
            return;
        }
    }
}
