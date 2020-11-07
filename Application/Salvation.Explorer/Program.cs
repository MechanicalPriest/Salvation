using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.HolyPriest;
using Salvation.Core.Profile;
using Salvation.Core.State;
using Salvation.Explorer.Modelling;
using Salvation.Utility.SpellDataUpdate;
using SimcProfileParser;
using System;

namespace Salvation.Explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSalvationCore();

                    // Explorer specific utility services
                    services.AddSingleton<IComparisonModeller<CovenantComparisonsResult>, CovenantComparisons>();

                    services.AddSingleton<IHolyPriestExplorer, HolyPriestExplorer>();

                    services.AddSingleton<ISpellDataUpdateService, SpellDataUpdateService>();
                    services.AddSingleton<ISpellDataService<HolyPriestSpellDataService>, HolyPriestSpellDataService>();

                    // Application service
                    services.AddHostedService(serviceProvider =>
                        new Explorer(
                            args,
                            serviceProvider.GetService<IHolyPriestExplorer>(),
                            serviceProvider.GetService<ISpellDataUpdateService>()));
                });
    }
}
