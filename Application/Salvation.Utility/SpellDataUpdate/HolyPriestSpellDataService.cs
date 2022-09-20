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
        private readonly ISimcGenerationService _simcGenerationService;
        private readonly IList<uint> _spells;

        public HolyPriestSpellDataService(ISimcGenerationService simcGenerationService)
        {
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
                (uint)Spell.PrayerOfMending,
                (uint)Spell.PrayerOfMendingBuff,
                (uint)Spell.PrayerOfMendingHeal,
                (uint)Spell.Renew,

                (uint)Spell.HolyFire,
                (uint)Spell.Smite,
                (uint)Spell.ShadowWordPain,

                // Talents
                (uint)Spell.Enlightenment,
                (uint)Spell.CosmicRipple,
                (uint)Spell.Halo,
                (uint)Spell.HaloHeal,
                (uint)Spell.HaloDamage,
                (uint)Spell.DivineStar,
                (uint)Spell.DivineStarHeal,
                (uint)Spell.DivineStarDamage,
                (uint)Spell.Benediction,
                (uint)Spell.HolyWordSalvation,

                // Spells
                (uint)Spell.PrayerOfHealing,
                (uint)Spell.HolyNova,
                (uint)Spell.HolyNovaRank2,
                (uint)Spell.CircleOfHealing,
                (uint)Spell.DivineHymn,
                (uint)Spell.HolyWordSanctify,
                (uint)Spell.HolyWordSerenity,
                (uint)Spell.GuardianSpirit,
                
                // DPS
                (uint)Spell.HolyWordChastise,
                (uint)Spell.ShadowWordDeath,

                #region Shadowlands spells
                
                // Covenant
                (uint)Spell.Mindgames,
                (uint)Spell.MindgamesHeal,

                // Legendaries
                (uint)Spell.HarmoniousApparatus,
                (uint)Spell.DivineImage,
                (uint)Spell.DivineImageHealingLight,
                (uint)Spell.DivineImageDazzlingLight,
                (uint)Spell.DivineImageSearingLight,
                (uint)Spell.DivineImageLightEruption,
                (uint)Spell.DivineImageBlessedLight,
                (uint)Spell.DivineImageTranquilLight,

                // Consumable


                // Trinket

                #endregion
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
                IntBase = 2501,
                StamBase = 1910,
                ManaBase = 250000, // __base_mp in sc_scale_data.inc
                // This is set to 1.0 as part of #159
                ArmorSkillsMultiplier = 1.00, // 5% extra main stat from Armor Skills

                // These come from __combat_ratings in sc_scale_data.inc
                CritCost = 220,
                HasteCost = 210,
                VersCost = 250, // Ver damage taken cost is 80
                MasteryCost = 176, // This is the 35 base cost * 0.80 holy priest modifier
                LeechCost = 132,
                SpeedCost = 62, 
                AvoidanceCost = 88,
                StamCost = 20
            };

            // Add the spells
            var spells = new List<BaseSpellData>();

            foreach (var spell in _spells)
            {
                // TODO: feed up level 60 from somewhere else. Now level 70.
                var spellOptions = new SimcSpellOptions()
                {
                    SpellId = spell,
                    PlayerLevel = 60
                };

                var spellData = await _simcGenerationService.GenerateSpellAsync(spellOptions);

                var newSpell = GetBaseSpellData(spellData);

                spells.Add(newSpell);
            }

            spec.Spells = spells;

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

            // Add the level 60 spellbudget value if it exists in the spelldata.
            if (effect.ScaleBudget != 0)
                newEffect.ScaleValues.Add(60, effect.ScaleBudget);

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
                case (uint)Spell.ShadowWordDeath:
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
