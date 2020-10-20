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
    public class UnholyNova : SpellService, IUnholyNovaSpellService
    {
        private readonly IUnholyTransfusionSpellService _unholyTransfuionSpellService;

        public UnholyNova(IGameStateService gameStateService,
            IModellingJournal journal,
            IUnholyTransfusionSpellService unholyTransfuionSpellService)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.UnholyNova;
            _unholyTransfuionSpellService = unholyTransfuionSpellService;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyNova);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            // Apply the transufion DoT/HoT
            var unholyTransfusionSpellData = _gameStateService.GetSpellData(gameState, Spell.UnholyTransfusion);

            var uhtResults = _unholyTransfuionSpellService.GetCastResults(gameState, unholyTransfusionSpellData);

            result.AdditionalCasts.Add(uhtResults);

            return result;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyNova);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            double averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyNova);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd)
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            var numTargets = base.GetNumberOfHealingTargets(gameState, spellData);

            if (numTargets == 0)
            {
                // UN stores is max targets in effect 844015 #2
                numTargets = spellData.GetEffect(844015).BaseValue;
            }

            return numTargets;
        }
    }
}
