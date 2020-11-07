using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
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
            // Common services
            builder.Services.AddSingleton<IConstantsService, ConstantsService>();
            builder.Services.AddSingleton<IGameStateService, GameStateService>();
            builder.Services.AddSingleton<IProfileService, ProfileService>();
            builder.Services.AddSingleton<IStatWeightGenerationService, StatWeightGenerator>();

            // Holy Priest specific services
            builder.Services.AddSingleton<IModellingService, HolyPriestModellingService>();

            // Spells
            builder.Services.AddSingleton<ISpellService<IFlashHealSpellService>, FlashHeal>();
            builder.Services.AddSingleton<ISpellService<IHolyWordSerenitySpellService>, HolyWordSerenity>();
            builder.Services.AddSingleton<ISpellService<IHolyWordSalvationSpellService>, HolyWordSalvation>();
            builder.Services.AddSingleton<ISpellService<IRenewSpellService>, Renew>();
            builder.Services.AddSingleton<ISpellService<IPrayerOfMendingSpellService>, PrayerOfMending>();
            builder.Services.AddSingleton<ISpellService<IPrayerOfHealingSpellService>, PrayerOfHealing>();
            builder.Services.AddSingleton<ISpellService<IHealSpellService>, Heal>();
            builder.Services.AddSingleton<ISpellService<IBindingHealSpellService>, BindingHeal>();
            builder.Services.AddSingleton<ISpellService<IHolyWordSanctifySpellService>, HolyWordSanctify>();
            builder.Services.AddSingleton<ISpellService<ICircleOfHealingSpellService>, CircleOfHealing>();
            builder.Services.AddSingleton<ISpellService<IDivineHymnSpellService>, DivineHymn>();
            builder.Services.AddSingleton<ISpellService<IDivineStarSpellService>, DivineStar>();
            builder.Services.AddSingleton<ISpellService<IHaloSpellService>, Halo>();
            builder.Services.AddSingleton<ISpellService<IHolyNovaSpellService>, HolyNova>();
            builder.Services.AddSingleton<ISpellService<IPowerWordShieldSpellService>, PowerWordShield>();
            // Covenants
            builder.Services.AddSingleton<ISpellService<IFaeGuardiansSpellService>, FaeGuardians>();
            builder.Services.AddSingleton<ISpellService<IMindgamesSpellService>, Mindgames>();
            builder.Services.AddSingleton<ISpellService<IUnholyNovaSpellService>, UnholyNova>();
            builder.Services.AddSingleton<ISpellService<IUnholyTransfusionSpellService>, UnholyTransfusion>();
            builder.Services.AddSingleton<ISpellService<IBoonOfTheAscendedSpellService>, BoonOfTheAscended>();
            builder.Services.AddSingleton<ISpellService<IAscendedBlastSpellService>, AscendedBlast>();
            builder.Services.AddSingleton<ISpellService<IAscendedNovaSpellService>, AscendedNova>();
            builder.Services.AddSingleton<ISpellService<IAscendedEruptionSpellService>, AscendedEruption>();

            // DPS
            builder.Services.AddSingleton<ISpellService<ISmiteSpellService>, Smite>();
            builder.Services.AddSingleton<ISpellService<IHolyWordChastiseSpellService>, HolyWordChastise>();
            builder.Services.AddSingleton<ISpellService<IShadowWordPainSpellService>, ShadowWordPain>();
            builder.Services.AddSingleton<ISpellService<IShadowWordDeathSpellService>, ShadowWordDeath>();
            builder.Services.AddSingleton<ISpellService<IHolyFireSpellService>, HolyFire>();

            // SimcProfileParser
            builder.Services.AddSimcProfileParser();
        }
    }
}
