using Microsoft.Extensions.DependencyInjection;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.Common.Consumables;
using Salvation.Core.Modelling.Common.Items;
using Salvation.Core.Modelling.Common.Traits;
using Salvation.Core.Modelling.HolyPriest;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using SimcProfileParser;
using System;

namespace Salvation.Core
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Register the dependencies for the entire core of Salvation
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSalvationCore(this IServiceCollection services)
        {
            // Common services
            services.AddSingleton<IConstantsService, ConstantsService>();
            services.AddSingleton<IGameStateService, GameStateService>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<ISimcProfileService, SimcProfileService>();
            services.AddSingleton<IStatWeightGenerationService, StatWeightGenerator>();

            // SpellFactoryService
            services.AddSingleton<ISpellServiceFactory>(serviceProvider =>
            {
                ISpellService spellFactoryFunc(Type type) => (ISpellService)serviceProvider.GetService(type);
                return new SpellServiceFactory(spellFactoryFunc);
            });

            // Core Spells
            services.AddCoreSpells();

            // Holy Priest specific services
            services.AddHolyPriestSpells();
            services.AddSingleton<IModellingService, HolyPriestModellingService>();

            // Simc profile parser library
            services.AddSimcProfileParser();

            return services;
        }

        /// <summary>
        /// Register the dependencies for the various core spells. Included as part of AddSalvationCore()
        /// </summary>
        public static IServiceCollection AddCoreSpells(this IServiceCollection services)
        {
            // Consumables
            services.AddSingleton<ISpellService<ISpectralFlaskOfPowerSpellService>, SpectralFlaskOfPower>();
            services.AddSingleton<ISpellService<ISpiritualManaPotionSpellService>, SpiritualManaPotion>();

            // Items
            services.AddSingleton<ISpellService<IUnboundChangelingSpellService>, UnboundChangeling>();

            // Covenant
            // - Kyrian Traits
            services.AddSingleton<ISpellService<ILetGoOfThePastSpellService>, LetGoOfThePast>();
            services.AddSingleton<ISpellService<ICombatMeditationSpellSerivce>, CombatMeditation>();
            services.AddSingleton<ISpellService<IPointedCourageSpellService>, PointedCourage>();
            services.AddSingleton<ISpellService<IValiantStrikesSpellService>, ValiantStrikes>();
            services.AddSingleton<ISpellService<IResonantAccoladesSpellService>, ResonantAccolades>();
            services.AddSingleton<ISpellService<IBronsCallToActionSpellService>, BronsCallToAction>();
            // - Necrolord Traits
            services.AddSingleton<ISpellService<IVolatileSolventSpellService>, VolatileSolvent>();
            services.AddSingleton<ISpellService<IUltimateFormSpellService>, UltimateForm>();

            return services;
        }

        /// <summary>
        /// Register the dependencies for the Holy Priest specific spells. Included as part of AddSalvationCore()
        /// </summary>
        public static IServiceCollection AddHolyPriestSpells(this IServiceCollection services)
        {
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
            services.AddSingleton<ISpellService<IFleshcraftSpellService>, Fleshcraft>();

            // DPS
            services.AddSingleton<ISpellService<ISmiteSpellService>, Smite>();
            services.AddSingleton<ISpellService<IHolyWordChastiseSpellService>, HolyWordChastise>();
            services.AddSingleton<ISpellService<IShadowWordPainSpellService>, ShadowWordPain>();
            services.AddSingleton<ISpellService<IShadowWordDeathSpellService>, ShadowWordDeath>();
            services.AddSingleton<ISpellService<IHolyFireSpellService>, HolyFire>();

            // Legendary Powers
            services.AddSingleton<ISpellService<IEchoOfEonarSpellSevice>, EchoOfEonar>();
            services.AddSingleton<ISpellService<ICauterizingShadowsSpellSevice>, CauterizingShadows>();

            return services;
        }
    }
}
