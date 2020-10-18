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
                (uint)Spell.AscendedBlast,
                (uint)Spell.Benediction,
                (uint)Spell.FlashHeal,
                (uint)Spell.CircleOfHealing,
            };
        }

        public async Task<BaseSpec> Generate()
        {
            var spec = new BaseSpec();
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
                // newSpell.ManaCost = ; // TODO: Need SimcProfileParser issue #46 resolved
                MaxRange = spell.MaxRange,
                //newSpell.NumberOfHealingTargets = spellData.MaxTargets; // TODO: Confirm this on a spell with MaxTargets set
                //newSpell.NumberOfDamageTargets = spellData.MaxTargets; // TODO: Confirm this on a spell with MaxTargets set
                BaseCastTime = spell.CastTime,
                //newSpell.IsCastTimeHasted = ; // TODO: Is this even stored anywhere? Probably not... but I think everything is hasted anyway. remove?
                BaseCooldown = spell.Cooldown,
                //newSpell.IsCooldownHasted = ; // TODO: No clue where this is stored? - stored in the Priest class aura.
                //newSpell.Duration = ; // So this is stored in the effects and can probably be removed.
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
    }
}
