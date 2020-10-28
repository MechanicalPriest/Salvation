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

                // Talents
                (uint)Spell.Enlightenment,
                (uint)Spell.CosmicRipple,
                (uint)Spell.BindingHeal,
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
                (uint)Spell.PrayerOfMendingRank2,
                (uint)Spell.PrayerOfMendingBuff,
                (uint)Spell.PrayerOfMendingHeal,
                (uint)Spell.EchoOfLight,

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

                // Legendaries
                (uint)Spell.HarmoniousApparatus,

                // Conduits
                (uint)Spell.CharitableSoul,
                (uint)Spell.CourageousAscension,
                (uint)Spell.FesteringTransfusion,
                (uint)Spell.FaeFermata,
                (uint)Spell.ShatteredPerceptions,
                (uint)Spell.HolyOration,
                
                // DPS
                (uint)Spell.Smite,
                (uint)Spell.SmiteRank2,
                (uint)Spell.HolyWordChastise,
                (uint)Spell.ShadowWordPain,
                (uint)Spell.ShadowWordPainRank2,
                (uint)Spell.ShadowWordDeath,
                (uint)Spell.ShadowWordDeathRank2,
                (uint)Spell.HolyFire,
            };
        }

        public async Task<BaseSpec> Generate()
        {
            var spec = new BaseSpec
            {
                Class = "Priest",
                Spec = "Holy",
                SpecId = 257,

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
                // TODO: feed up level 60 from somewhere else
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
                Duration = spell.Duration,
                Gcd = spell.Gcd / 1000d,
                ConduitRanks = spell.ConduitRanks
                //newSpell.IsMasteryTriggered = ; // So this another weird one. Anything that has a healing effect of type 10 (Direct Heal) seems to proc it.
            };

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
                TriggerSpellid = effect.TriggerSpellId,
                Amplitude = effect.Amplitude,
                TriggerSpell = GetBaseSpellData(effect.TriggerSpell),
                Type = effect.EffectType
            };

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
                default:
                    break;
            }
        }
    }
}
