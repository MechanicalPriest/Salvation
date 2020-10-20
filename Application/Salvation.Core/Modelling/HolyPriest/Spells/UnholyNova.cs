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

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyNova);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            decimal averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * (decimal)GetNumberOfHealingTargets(gameState, spellData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyNova);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            decimal maximumPotentialCasts = 60m / (hastedCastTime + hastedCd)
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
