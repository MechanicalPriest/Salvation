using Microsoft.Extensions.Logging;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using SimcProfileParser.Interfaces;
using SimcProfileParser.Model.Generated;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Salvation.Utility.SpellDataUpdate
{
    public class HolyPriestSpellDataService : SpellDataService,
        ISpellDataService<HolyPriestSpellDataService>
    {
        private readonly ILogger<ISpellDataService<HolyPriestSpellDataService>> _logger;
        private readonly ISimcGenerationService _simcGenerationService;
        private readonly IList<uint> _spells;

        private static uint PLAYER_LEVEL = 70;

        public HolyPriestSpellDataService(ILogger<ISpellDataService<HolyPriestSpellDataService>> logger,
            ISimcGenerationService simcGenerationService)
        {
            _logger = logger;
            _simcGenerationService = simcGenerationService;
            _spells = new List<uint>()
            {
                // Baseline Spells
                (uint)Spell.HolyPriest,
                (uint)Spell.Priest,
                (uint)Spell.LeechHeal,
                (uint)Spell.EchoOfLight,
                (uint)Spell.Heal,
                (uint)Spell.FlashHeal,
                (uint)Spell.PowerWordShield,
                (uint)Spell.HolyFire,
                (uint)Spell.Smite,
                (uint)Spell.ShadowWordPain,

                // Talents - Priest
                (uint)Spell.PrayerOfMending,
                (uint)Spell.Renew,
                (uint)Spell.Halo,
                (uint)Spell.DivineStar,
                (uint)Spell.HolyNova,
                (uint)Spell.ShadowWordDeath,
                (uint)Spell.Mindgames,
                (uint)Spell.ImprovedFlashHeal,
                (uint)Spell.FocusedMending,
                (uint)Spell.TwistOfFate,
                (uint)Spell.UnwaveringWill,

                // Talent - Priest supporting spells
                (uint)Spell.PrayerOfMendingBuff,
                (uint)Spell.PrayerOfMendingHeal,
                (uint)Spell.HaloHeal,
                (uint)Spell.HaloDamage,
                (uint)Spell.DivineStarHeal,
                (uint)Spell.DivineStarDamage,
                (uint)Spell.MindgamesHeal,

                // Talents - Holy
                (uint)Spell.Enlightenment,
                (uint)Spell.CosmicRipple,
                (uint)Spell.Benediction,
                (uint)Spell.HolyWordSalvation,
                (uint)Spell.PrayerOfHealing,
                (uint)Spell.CircleOfHealing,
                (uint)Spell.DivineHymn,
                (uint)Spell.HolyWordSanctify,
                (uint)Spell.HolyWordSerenity,
                (uint)Spell.GuardianSpirit,
                (uint)Spell.HarmoniousApparatus,
                (uint)Spell.DivineImage,
                (uint)Spell.HolyWordChastise,
                (uint)Spell.CrisisManagement,
                (uint)Spell.PrayerfulLitany,
                (uint)Spell.Orison,
                (uint)Spell.GalesOfSong,
                (uint)Spell.PrayersOfTheVirtuous,
                (uint)Spell.PrismaticEchoes,
                (uint)Spell.RapidRecovery,
                (uint)Spell.ResonantWords,
                (uint)Spell.EverlastingLight,
                (uint)Spell.Pontifex,
                (uint)Spell.SayYourPrayers,
                (uint)Spell.DivineService,
                (uint)Spell.TrailOfLight,
                (uint)Spell.PrayerCircle,
                (uint)Spell.BindingHeals,

                // Talent - Holy supporting spells
                (uint)Spell.CosmicRippleHeal,
                (uint)Spell.DivineImageHealingLight,
                (uint)Spell.DivineImageDazzlingLight,
                (uint)Spell.DivineImageSearingLight,
                (uint)Spell.DivineImageLightEruption,
                (uint)Spell.DivineImageBlessedLight,
                (uint)Spell.DivineImageTranquilLight,
                (uint)Spell.TrailOfLightHeal,
                (uint)Spell.SanctifiedPrayersBuff,
                (uint)Spell.PrayerCircleBuff,
                (uint)Spell.BindingHealsHeal,
            };
        }

        public async Task<BaseSpec> Generate()
        {
            var spec = new BaseSpec
            {
                Class = "Priest",
                Spec = "Holy",
                SpecId = 257,
                GCDFloor = 0.75,

                CritMultiplier = 2,
                CritBase = 0.05,
                HasteBase = 0.0,
                VersBase = 0.0,
                MasteryBase = 0.1,
                IntBase = 2089, // From a human/panda in-game
                StamBase = 1599,
                ManaBase = 250000, // __base_mp in sc_scale_data.inc
                // This is set to 1.0 as part of #159
                ArmorSkillsMultiplier = 1.00, // 5% extra main stat from Armor Skills

                // These come from __combat_ratings in sc_scale_data.inc
                CritCost = 180,
                HasteCost = 170,
                VersCost = 205, // Ver damage taken cost is double
                MasteryCost = 144, // This is the base cost * 0.80 holy priest modifier
                LeechCost = 110,
                SpeedCost = 50, 
                AvoidanceCost = 72,
                StamCost = 20
            };

            _logger?.LogTrace("Generating SpellData for {class} {spec}", spec.Class, spec.Spec);

            // Add the spells
            var spells = new List<BaseSpellData>();

            foreach (var spell in _spells)
            {
                var spellOptions = new SimcSpellOptions()
                {
                    SpellId = spell,
                    PlayerLevel = PLAYER_LEVEL
                };

                var spellData = await _simcGenerationService.GenerateSpellAsync(spellOptions);

                var newSpell = GetBaseSpellData(spellData);

                _logger?.LogTrace("Adding spell {spellId}: {spellname}", newSpell.Id, newSpell.Name);

                spells.Add(newSpell);
            }

            spec.Spells = spells;

            _logger?.LogTrace("Done generating spelldata.");

            return spec;
        }

        internal BaseSpellData GetBaseSpellData(SimcSpell spell)
        {
            if (spell == null)
                return null;

            var newSpell = new BaseSpellData
            {
                Id = spell.SpellId,
                Name = spell.Name,
                MaxRange = spell.MaxRange,
                BaseCastTime = spell.CastTime,
                BaseCooldown = spell.Cooldown,
                ChargeCooldown = spell.ChargeCooldown,
                Charges = spell.Charges,
                MaxStacks = spell.MaxStacks,
                Duration = spell.Duration,
                Gcd = spell.Gcd / 1000d,
                Rppm = spell.Rppm,
                ProcChance = spell.ProcChance,
                InternalCooldown = spell.InternalCooldown
            };

            // Check if RPPM is modified by spec or haste
            foreach(var rppmMod in spell.RppmModifiers)
            {
                if (rppmMod.RppmIsSpecModified && rppmMod.RppmSpec == (uint)Spec.HolyPriest)
                    newSpell.Rppm *= rppmMod.RppmCoefficient;

                if (rppmMod.RppmIsHasted)
                    newSpell.RppmIsHasted = true;
            }

            double manacost = 0;

            if (spell.PowerCosts != null && spell.PowerCosts.Count > 0)
            {
                if (spell.PowerCosts.ContainsKey(0))
                {
                    manacost = spell.PowerCosts[0];
                }

                foreach (var PowerCost in spell.PowerCosts)
                {
                    if (PowerCost.Key.Equals((uint)Spell.HolyPriest))
                    {
                        manacost = PowerCost.Value;
                        break;
                    }
                }
            }

            newSpell.ManaCost = manacost;

            foreach (var effect in spell.Effects)
            {
                var newEffect = GetBaseSpellDataEffect(effect);

                if (newEffect != null)
                    newSpell.Effects.Add(newEffect);
            }

            ApplyOverrides(newSpell);

            return newSpell;
        }

        internal BaseSpellDataEffect GetBaseSpellDataEffect(SimcSpellEffect effect)
        {
            if (effect == null)
                return null;

            var newEffect = new BaseSpellDataEffect()
            {
                Id = effect.Id,
                BaseValue = effect.BaseValue,
                SpCoefficient = effect.SpCoefficient,
                Coefficient = effect.Coefficient,
                TriggerSpellid = effect.TriggerSpellId,
                Amplitude = effect.Amplitude,
                TriggerSpell = GetBaseSpellData(effect.TriggerSpell),
                Type = effect.EffectType,
            };

            // Add the level spellbudget value if it exists in the spelldata.
            if (effect.ScaleBudget != 0)
                newEffect.ScaleValues.Add((int)PLAYER_LEVEL, effect.ScaleBudget);

            return newEffect;
        }

        /// <summary>
        /// Apply overrides to specific spells as needed. This is either for spells that the 
        /// </summary>
        /// <param name="baseSpellData"></param>
        /// <returns></returns>
        internal void ApplyOverrides(BaseSpellData baseSpellData)
        {
            switch (baseSpellData.Id)
            {
                case (uint)Spell.CircleOfHealing:
                case (uint)Spell.PrayerOfMending:
                case (uint)Spell.PowerWordShield:
                    // This comes from the Priest aura 137030 effect #1 179714
                    baseSpellData.IsCooldownHasted = true;
                    break;
                case (uint)Spell.DivineImageBlessedLight:
                    // Blessed light's spelldata for targets is now in Effect #1 288952 as part of chain targets
                    // We don't get chain targets through yet, so set it manually
                    baseSpellData.GetEffect(288952).BaseValue = 5;
                    break;
                default:
                    break;
            }
        }
    }
}
