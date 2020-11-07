using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.HolyPriest;
using Salvation.Core.Modelling.HolyPriest.Spells;
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
                    // Common services
                    services.AddSingleton<IConstantsService, ConstantsService>();
                    services.AddSingleton<IGameStateService, GameStateService>();
                    services.AddSingleton<IProfileService, ProfileService>();
                    services.AddSingleton<IComparisonModeller<CovenantComparisonsResult>, CovenantComparisons>();
                    services.AddSingleton<IStatWeightGenerationService, StatWeightGenerator>();

                    // SpellFactoryService
                    services.AddSingleton<ISpellServiceFactory>(serviceProvider =>
                    {
                        Func<Type, ISpellService> spellFactoryFunc = type => (ISpellService)serviceProvider.GetService(type);
                        return new SpellServiceFactory(spellFactoryFunc);
                    });

                    // Holy Priest specific services
                    services.AddSingleton<IHolyPriestExplorer, HolyPriestExplorer>();
                    services.AddSingleton<IModellingService, HolyPriestModellingService>();

                    // Spells
                    services.AddSingleton<ISpellService<IFlashHealSpellService>, FlashHeal>();
                    services.AddSingleton<ISpellService<IHolyWordSerenitySpellService>, HolyWordSerenity>();
                    services.AddSingleton<ISpellService<IHolyWordSalvationSpellService>, HolyWordSalvation>();
                    services.AddSingleton<ISpellService<IRenewSpellService>, Renew>();
                    services.AddSingleton<ISpellService<IPrayerOfMendingSpellService>, PrayerOfMending>();
                    services.AddSingleton<ISpellService<IPrayerOfHealingSpellService>, PrayerOfHealing>();
                    services.AddSingleton<ISpellService<IHealSpellService>, Heal>();
                    services.AddSingleton<ISpellService<IBindingHealSpellService>, BindingHeal>();
                    services.AddSingleton<ISpellService<IHolyWordSanctifySpellService>, HolyWordSanctify>();
                    services.AddSingleton<ISpellService<ICircleOfHealingSpellService>, CircleOfHealing>();
                    services.AddSingleton<ISpellService<IDivineHymnSpellService>, DivineHymn>();
                    services.AddSingleton<ISpellService<IDivineStarSpellService>, DivineStar>();
                    services.AddSingleton<ISpellService<IHaloSpellService>, Halo>();
                    services.AddSingleton<ISpellService<IHolyNovaSpellService>, HolyNova>();
                    services.AddSingleton<ISpellService<IPowerWordShieldSpellService>, PowerWordShield>();
                    // Covenants
                    services.AddSingleton<ISpellService<IFaeGuardiansSpellService>, FaeGuardians>();
                    services.AddSingleton<ISpellService<IMindgamesSpellService>, Mindgames>();
                    services.AddSingleton<ISpellService<IUnholyNovaSpellService>, UnholyNova>();
                    services.AddSingleton<ISpellService<IUnholyTransfusionSpellService>, UnholyTransfusion>();
                    services.AddSingleton<ISpellService<IBoonOfTheAscendedSpellService>, BoonOfTheAscended>();
                    services.AddSingleton<ISpellService<IAscendedBlastSpellService>, AscendedBlast>();
                    services.AddSingleton<ISpellService<IAscendedNovaSpellService>, AscendedNova>();
                    services.AddSingleton<ISpellService<IAscendedEruptionSpellService>, AscendedEruption>();

                    // DPS
                    services.AddSingleton<ISpellService<ISmiteSpellService>, Smite>();
                    services.AddSingleton<ISpellService<IHolyWordChastiseSpellService>, HolyWordChastise>();
                    services.AddSingleton<ISpellService<IShadowWordPainSpellService>, ShadowWordPain>();
                    services.AddSingleton<ISpellService<IShadowWordDeathSpellService>, ShadowWordDeath>();
                    services.AddSingleton<ISpellService<IHolyFireSpellService>, HolyFire>();

                    // Utility services
                    services.AddSingleton<ISpellDataUpdateService, SpellDataUpdateService>();
                    services.AddSingleton<ISpellDataService<HolyPriestSpellDataService>, HolyPriestSpellDataService>();

                    services.AddSimcProfileParser();

                    // Application service
                    services.AddHostedService<Explorer>(serviceProvider =>
                        new Explorer(
                            args,
                            serviceProvider.GetService<IHolyPriestExplorer>(),
                            serviceProvider.GetService<ISpellDataUpdateService>()));
                });
    }
}
