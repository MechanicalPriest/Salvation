using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using SimcProfileParser.Interfaces;
using SimcProfileParser.Model.Generated;
using System;
using System.Collections.Generic;
using System.Text;
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
                // Talents
                (uint)Spell.Enlightenment,
                (uint)Spell.CosmicRipple,
                (uint)Spell.BindingHeal,
                (uint)Spell.Halo,
                (uint)Spell.DivineStar,
                (uint)Spell.Benediction,
                (uint)Spell.HolyWordSalvation,

                // Spells
                (uint)Spell.Heal,
                (uint)Spell.FlashHeal,
                (uint)Spell.PrayerOfHealing,
                (uint)Spell.HolyNova,
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

                // Covenant
                (uint)Spell.Mindgames,
                (uint)Spell.FaeGuardians,
                (uint)Spell.BenevolentFaerie,
                (uint)Spell.GuardianFaerie,
                (uint)Spell.BoonOfTheAscended,
                (uint)Spell.AscendedNova,
                (uint)Spell.AscendedBlast,
                (uint)Spell.AscendedEruption,
                (uint)Spell.UnholyNova,
                (uint)Spell.UnholyTransfusion,
                (uint)Spell.UnholyTransfusionDebuff,

                // Legendaries
                (uint)Spell.HarmoniousApparatus,

                // Conduits
                (uint)Spell.CharitableSoul,
                (uint)Spell.CourageousAscension,
                (uint)Spell.FesteringTransfusion,
                (uint)Spell.FaeFermata,
                (uint)Spell.ShatteredPerceptions,
                (uint)Spell.HolyOration,
            };
        }

        public async Task<BaseSpec> Generate()
        {
            var spec = new BaseSpec();
            spec.Class = "Priest";
            spec.Spec = "Holy";
            spec.SpecId = 257;

            spec.CritBase = 0.05;
            spec.HasteBase = 0.0;
            spec.VersBase = 0.0;
            spec.MasteryBase = 0.1;
            spec.IntBase = 450;
            spec.StamBase = 416;
            spec.ManaBase = 50000; // __base_mp in sc_scale_data.inc

            // These come from __combat_ratings in sc_scale_data.inc
            spec.CritCost = 35;
            spec.HasteCost = 33;
            spec.VersCost = 40; // Ver damage taken cost is 80
            spec.MasteryCost = 28; // This is the 35 base cost * 0.80 holy priest modifier
            spec.LeechCost = 21;
            spec.SpeedCost = 10;
            spec.AvoidanceCost = 14;
            spec.StamCost = 20;

            // Add the spells
            var spells = new List<BaseSpellData>();

            foreach(var spell in _spells)
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
                ManaCost = spell.PowerCost,
                MaxRange = spell.MaxRange,
                BaseCastTime = spell.CastTime,
                BaseCooldown = spell.Cooldown,
                Duration = spell.Duration,
                Gcd = spell.Gcd / 1000
                //newSpell.Coeff1; // This and coeff 2 and 3 make way for the spell effect data.
                //newSpell.IsMasteryTriggered = ; // So this another weird one. Anything that has a healing effect of type 10 (Direct Heal) seems to proc it.
            };
            
            foreach (var effect in spell.Effects)
            {
                var newEffect = GetBaseSpellDataEffect(effect);

                if(newEffect != null)
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
                TriggerSpell = GetBaseSpellData(effect.TriggerSpell)
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
                    // This comes from the Priest aura 137030 effect #1 179714
                    baseSpellData.IsCooldownHasted = true;
                    break;
                default:
                    break;
            }
        }
    }
}
