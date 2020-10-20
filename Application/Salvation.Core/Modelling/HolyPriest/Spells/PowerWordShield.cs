using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PowerWordShield : SpellService, IPowerWordShieldSpellService
    {
        public PowerWordShield(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.PowerWordShield;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            if (_gameStateService.IsConduitActive(gameState, Conduit.CharitableSoul))
            {
                var csSpellData = _gameStateService.GetConduitData(gameState, Conduit.CharitableSoul);

                // Turn the rank value into a multiplier. "Rank" 10 = 0.10
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.CharitableSoul);
                var rankMulti = csSpellData.Ranks[rank] / 100;

                AveragedSpellCastResult csComponent = new AveragedSpellCastResult();
                csComponent.SpellId = csSpellData.Id;
                csComponent.SpellName = csSpellData.Name;
                csComponent.RawHealing = result.RawHealing * rankMulti;
                csComponent.Healing = result.Healing * rankMulti;
                csComponent.Cooldown = 0;
                csComponent.Duration = 0;
                csComponent.Gcd = 0;
                csComponent.ManaCost = 0;
                csComponent.NumberOfHealingTargets = 1;
                csComponent.MakeSpellHaveNoCasts();

                result.AdditionalCasts.Add(csComponent);
            }

            return result;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.PowerWordShield);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            // Flash Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            double averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus
                * 2; // PW:S has a x2 SP% multiplier built in to it

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.PowerWordShield);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            double fillerCastTime = hastedCastTime == 0d
                ? hastedGcd
                : hastedCastTime;

            double maximumPotentialCasts = 60d / fillerCastTime;

            return maximumPotentialCasts;
        }

        public override double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            var numTargets = base.GetNumberOfHealingTargets(gameState, spellData);

            if (numTargets == 0)
                numTargets = 1;

            return numTargets;
        }
    }
}
