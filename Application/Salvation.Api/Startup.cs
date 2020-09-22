using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Models.HolyPriest.Spells;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(Salvation.Api.Startup))]
namespace Salvation.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {            
            // Common services
            builder.Services.AddSingleton<IConstantsService, ConstantsService>();
            builder.Services.AddSingleton<IGameStateService, GameStateService>();

            // Holy Priest specific services
            builder.Services.AddSingleton<IModellingService, HolyPriestModellingService>();
            // Spells
            builder.Services.AddSingleton<IFlashHealSpellService, FlashHeal>();
            builder.Services.AddSingleton<IHolyWordSerenitySpellService, HolyWordSerenity>();            
            builder.Services.AddSingleton<IHolyWordSalvationSpellService, HolyWordSalvation>();            
            builder.Services.AddSingleton<IRenewSpellService, Renew>();            
            builder.Services.AddSingleton<IPrayerOfMendingSpellService, PrayerOfMending>();
            builder.Services.AddSingleton<IPrayerOfHealingSpellService, PrayerOfHealing>();
        }
    }
}
