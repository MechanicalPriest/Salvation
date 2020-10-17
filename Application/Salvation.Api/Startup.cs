using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Salvation.Api.Services;
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
using SimcProfileParser;

[assembly: FunctionsStartup(typeof(Salvation.Api.Startup))]
namespace Salvation.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Api services
            builder.Services.AddSingleton<IApiResultService, ApiResultService>();

            // Common services
            builder.Services.AddSingleton<IConstantsService, ConstantsService>();
            builder.Services.AddSingleton<IGameStateService, GameStateService>();
            builder.Services.AddSingleton<IModellingJournal, ModellingJournal>();
            builder.Services.AddSingleton<IProfileGenerationService, ProfileGenerationService>();
            builder.Services.AddSingleton<IStatWeightGenerationService, StatWeightGenerator>();

            // Holy Priest specific services
            builder.Services.AddSingleton<IModellingService, HolyPriestModellingService>();

            // Spells
            builder.Services.AddSingleton<IFlashHealSpellService, FlashHeal>();
            builder.Services.AddSingleton<IHolyWordSerenitySpellService, HolyWordSerenity>();
            builder.Services.AddSingleton<IHolyWordSalvationSpellService, HolyWordSalvation>();
            builder.Services.AddSingleton<IRenewSpellService, Renew>();
            builder.Services.AddSingleton<IPrayerOfMendingSpellService, PrayerOfMending>();
            builder.Services.AddSingleton<IPrayerOfHealingSpellService, PrayerOfHealing>();
            builder.Services.AddSingleton<IHealSpellService, Heal>();
            builder.Services.AddSingleton<IBindingHealSpellService, BindingHeal>();
            builder.Services.AddSingleton<IHolyWordSanctifySpellService, HolyWordSanctify>();
            builder.Services.AddSingleton<ICircleOfHealingSpellService, CircleOfHealing>();
            builder.Services.AddSingleton<IDivineHymnSpellService, DivineHymn>();
            builder.Services.AddSingleton<IDivineStarSpellService, DivineStar>();
            builder.Services.AddSingleton<IHaloSpellService, Halo>();
            builder.Services.AddSingleton<IHolyNovaSpellService, HolyNova>();
            builder.Services.AddSingleton<IPowerWordShieldSpellService, PowerWordShield>();

            // Covenants
            builder.Services.AddSingleton<IFaeGuardiansSpellService, FaeGuardians>();
            builder.Services.AddSingleton<IMindgamesSpellService, Mindgames>();
            builder.Services.AddSingleton<IUnholyNovaSpellService, UnholyNova>();
            builder.Services.AddSingleton<IUnholyTransfusionSpellService, UnholyTransfusion>();
            builder.Services.AddSingleton<IBoonOfTheAscendedSpellService, BoonOfTheAscended>();
            builder.Services.AddSingleton<IAscendedBlastSpellService, AscendedBlast>();
            builder.Services.AddSingleton<IAscendedNovaSpellService, AscendedNova>();
            builder.Services.AddSingleton<IAscendedEruptionSpellService, AscendedEruption>();

            // SimcProfileParser
            builder.Services.AddSimcProfileParser();
        }
    }
}
