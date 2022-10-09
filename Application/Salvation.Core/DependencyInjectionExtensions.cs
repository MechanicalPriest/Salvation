using Microsoft.Extensions.DependencyInjection;
using Salvation.Core.Constants;
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
            
            // Items

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
            services.AddSingleton<ISpellService<IHolyWordSanctifySpellService>, HolyWordSanctify>();
            services.AddSingleton<ISpellService<ICircleOfHealingSpellService>, CircleOfHealing>();
            services.AddSingleton<ISpellService<IDivineHymnSpellService>, DivineHymn>();
            services.AddSingleton<ISpellService<IDivineStarSpellService>, DivineStar>();
            services.AddSingleton<ISpellService<IHaloSpellService>, Halo>();
            services.AddSingleton<ISpellService<IHolyNovaSpellService>, HolyNova>();
            services.AddSingleton<ISpellService<IPowerWordShieldSpellService>, PowerWordShield>();
            services.AddSingleton<ISpellService<IGuardianSpiritSpellService>, GuardianSpirit>();

            // Talents
            services.AddSingleton<ISpellService<ICosmicRippleSpellService>, CosmicRipple>();

            // Covenants
            services.AddSingleton<ISpellService<IMindgamesSpellService>, Mindgames>();

            // DPS
            services.AddSingleton<ISpellService<ISmiteSpellService>, Smite>();
            services.AddSingleton<ISpellService<IHolyWordChastiseSpellService>, HolyWordChastise>();
            services.AddSingleton<ISpellService<IShadowWordPainSpellService>, ShadowWordPain>();
            services.AddSingleton<ISpellService<IShadowWordDeathSpellService>, ShadowWordDeath>();
            services.AddSingleton<ISpellService<IHolyFireSpellService>, HolyFire>();

            // Legendary Powers
            services.AddSingleton<ISpellService<IDivineImageSpellService>, DivineImage>();
            services.AddSingleton<ISpellService<IDivineImageHealingLightSpellService>, DivineImageHealingLight>();
            services.AddSingleton<ISpellService<IDivineImageTranquilLightSpellService>, DivineImageTranquilLight>();
            services.AddSingleton<ISpellService<IDivineImageDazzlingsLightSpellService>, DivineImageDazzlingLights>();
            services.AddSingleton<ISpellService<IDivineImageBlessedLightSpellService>, DivineImageBlessedLight>();

            return services;
        }
    }
}
