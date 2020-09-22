using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Models.HolyPriest.Spells;
using Salvation.Core.State;
using Salvation.Explorer.Modelling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Salvation.Explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Common services
                    services.AddSingleton<IConstantsService, ConstantsService>();
                    services.AddSingleton<IGameStateService, GameStateService>();
                    services.AddSingleton<IModellingJournal, ModellingJournal>();

                    // Holy Priest specific services
                    services.AddSingleton<IHolyPriestExplorer, HolyPriestExplorer>();
                    services.AddSingleton<IModellingService, HolyPriestModellingService>();

                    // Spells
                    services.AddSingleton<IFlashHealSpellService, FlashHeal>();
                    services.AddSingleton<IHolyWordSerenitySpellService, HolyWordSerenity>();
                    services.AddSingleton<IHolyWordSalvationSpellService, HolyWordSalvation>();
                    services.AddSingleton<IRenewSpellService, Renew>();
                    services.AddSingleton<IPrayerOfMendingSpellService, PrayerOfMending>();
                    services.AddSingleton<IPrayerOfHealingSpellService, PrayerOfHealing>();
                    services.AddSingleton<IHealSpellService, Heal>();
                    services.AddSingleton<IBindingHealSpellService, BindingHeal>();
                    services.AddSingleton<IHolyWordSanctifySpellService, HolyWordSanctify>();
                    services.AddSingleton<ICircleOfHealingSpellService, CircleOfHealing>();
                    services.AddSingleton<IDivineHymnSpellService, DivineHymn>();
                    services.AddSingleton<IDivineStarSpellService, DivineStar>();

                    // Application service
                    services.AddHostedService<Explorer>();
                });
    }
}
