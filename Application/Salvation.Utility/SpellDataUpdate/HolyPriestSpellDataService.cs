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
                (uint)Spell.HolyPriest,
                (uint)Spell.Priest,
                (uint)Spell.LeechHeal,

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
                (uint)Spell.Heal,
                (uint)Spell.FlashHeal,
                (uint)Spell.PrayerOfHealing,
                (uint)Spell.HolyNova,
                (uint)Spell.HolyNovaRank2,
                (uint)Spell.CircleOfHealing,
                (uint)Spell.Renew,
                (uint)Spell.PowerWordShield,
                (uint)Spell.DivineHymn,
                (uint)Spell.HolyWordSanctify,
                (uint)Spell.HolyWordSerenity,
                (uint)Spell.PrayerOfMending,
                (uint)Spell.PrayerOfMendingBuff,
                (uint)Spell.PrayerOfMendingHeal,
                (uint)Spell.EchoOfLight,
                (uint)Spell.GuardianSpirit,

                // Covenant
                (uint)Spell.Mindgames,
                (uint)Spell.MindgamesHeal,
                (uint)Spell.FaeGuardians,
                (uint)Spell.BenevolentFaerie,
                (uint)Spell.GuardianFaerie,
                (uint)Spell.BoonOfTheAscended,
                (uint)Spell.AscendedNova,
                (uint)Spell.AscendedBlast,
                (uint)Spell.AscendedBlastHeal,
                (uint)Spell.AscendedEruption,
                (uint)Spell.UnholyNova,
                (uint)Spell.UnholyTransfusion,
                (uint)Spell.UnholyTransfusionDoT,
                (uint)Spell.Fleshcraft,

                // Legendaries
                (uint)Spell.HarmoniousApparatus,
                (uint)Spell.EchoOfEonar,
                (uint)Spell.EchoOfEonarHealingBuffSelf,
                (uint)Spell.CauterizingShadows,
                (uint)Spell.CauterizingShadowsHeal,
                (uint)Spell.FlashConcentration,
                (uint)Spell.DivineImage,
                (uint)Spell.DivineImageHealingLight,
                (uint)Spell.DivineImageDazzlingLight,
                (uint)Spell.DivineImageSearingLight,
                (uint)Spell.DivineImageLightEruption,
                (uint)Spell.DivineImageBlessedLight,
                (uint)Spell.DivineImageTranquilLight,

                // Conduits
                (uint)Spell.CharitableSoul,
                (uint)Spell.CourageousAscension,
                (uint)Spell.FesteringTransfusion,
                (uint)Spell.FaeFermata,
                (uint)Spell.ShatteredPerceptions,
                (uint)Spell.HolyOration,
                (uint)Spell.FocusedMending,
                (uint)Spell.ResonantWords,
                (uint)Spell.LastingSpirit,
                
                // DPS
                (uint)Spell.Smite,
                (uint)Spell.HolyWordChastise,
                (uint)Spell.ShadowWordPain,
                (uint)Spell.ShadowWordPainRank2,
                (uint)Spell.ShadowWordDeath,
                (uint)Spell.ShadowWordDeathRank2,
                (uint)Spell.HolyFire,

                // Consumable
                (uint)Spell.SpectralFlaskOfPower,
                (uint)Spell.SpiritualManaPotion,

                // Trinket
                (uint)Spell.UnboundChangeling,
                (uint)Spell.UnboundChangelingBuff,
                (uint)Spell.CabalistsHymnal,
                (uint)Spell.CabalistsHymnalBuff,
                (uint)Spell.SoullettingRuby,
                (uint)Spell.SoullettingRubyBuff,
                (uint)Spell.SoullettingRubyHeal,
                (uint)Spell.SoullettingRubyTrigger,
                (uint)Spell.ManaboundMirror,
                (uint)Spell.ManaboundMirrorBuff,
                (uint)Spell.ManaboundMirrorHeal,
                (uint)Spell.MacabreSheetMusic,
                (uint)Spell.MacabreSheetMusicTrigger,
                (uint)Spell.MacabreSheetMusicBuff,
                (uint)Spell.TuftOfSmolderingPlumage,
                (uint)Spell.TuftOfSmolderingPlumageBuff,
                (uint)Spell.ConsumptiveInfusion,
                (uint)Spell.ConsumptiveInfusionBuff,
                (uint)Spell.ConsumptiveInfusionDebuff,
                (uint)Spell.DarkmoonDeckRepose,
                (uint)Spell.DarkmoonDeckReposeAce,
                (uint)Spell.DarkmoonDeckReposeEight,
                (uint)Spell.VialOfSpectralEssence,
                (uint)Spell.OverflowingAnimaCage,
                (uint)Spell.OverflowingAnimaCageBuff,
                (uint)Spell.SiphoningPhylacteryShard,
                (uint)Spell.SiphoningPhylacteryShardBuff,

                // Trait
                // Kyrian
                (uint)Spell.CombatMeditation,
                (uint)Spell.CombatMeditationBuff,
                (uint)Spell.CombatMeditationExtension,
                (uint)Spell.LetGoOfThePast,
                (uint)Spell.LetGoOfThePastBuff,
                (uint)Spell.PointedCourage,
                (uint)Spell.PointedCourageBuff,
                (uint)Spell.ValiantStrikes,
                (uint)Spell.ValiantStrikesBuff,
                (uint)Spell.ResonantAccolades,
                (uint)Spell.ResonantAccoladesBuff,
                (uint)Spell.BronsCallToAction,
                // Necrolord
                (uint)Spell.VolatileSolvent,
                (uint)Spell.VolatileSolventHumanoid,
                (uint)Spell.VolatileSolventBeast,
                (uint)Spell.VolatileSolventDragonkin,
                (uint)Spell.VolatileSolventElemental,
                //(uint)Spell.VolatileSolventMechanical, // TOOD: Not coming through on spelldata
                (uint)Spell.UltimateForm,
                (uint)Spell.UltimateFormHeal,
                (uint)Spell.LeadByExample,
                (uint)Spell.LeadByExampleBuff,
                (uint)Spell.MarrowedGemstone,
                (uint)Spell.MarrowedGemstoneStacks,
                (uint)Spell.MarrowedGemstoneBuff,
                (uint)Spell.MarrowedGemstoneCooldown,
                (uint)Spell.ForgeborneReveries,
                (uint)Spell.ForgeborneReveriesBuff,
                // Night Fae
                (uint)Spell.GroveInvigoration,
                (uint)Spell.GroveInvigorationAnimaBuff,
                (uint)Spell.FieldOfBlossoms,
                (uint)Spell.FieldOfBlossomsEffect,
                (uint)Spell.FieldOfBlossomsBuff,
                (uint)Spell.NiyasToolsHerbs,
                (uint)Spell.NiyasToolsHerbsBuff,
                // Venthyr
                (uint)Spell.ThrillSeeker,
                (uint)Spell.ThrillSeekerBuff,
                (uint)Spell.ThrillSeekerStacks,
                (uint)Spell.SoothingShade,
                (uint)Spell.SoothingShadeBuff,
                (uint)Spell.SoothingShadeEffect,
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
                IntBase = 450,
                StamBase = 416,
                ManaBase = 50000, // __base_mp in sc_scale_data.inc

                // These come from __combat_ratings in sc_scale_data.inc
                CritCost = 35,
                HasteCost = 33,
                VersCost = 40, // Ver damage taken cost is 80
                MasteryCost = 28, // This is the 35 base cost * 0.80 holy priest modifier
                LeechCost = 21,
                SpeedCost = 10,
                AvoidanceCost = 14,
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
                ConduitRanks = spell.ConduitRanks,
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
