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

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.PowerWordShield);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            // Flash Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus
                * 2; // PW:S has a x2 SP% multiplier built in to it

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * (decimal)GetNumberOfHealingTargets(gameState, spellData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.PowerWordShield);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            decimal fillerCastTime = hastedCastTime == 0
                ? hastedGcd
                : hastedCastTime;

            decimal maximumPotentialCasts = 60m / fillerCastTime;

            return maximumPotentialCasts;
        }
    }
}
