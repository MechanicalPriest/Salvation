using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest.Spells
{
    public class UnholyNova : SpellService, IUnholyNovaSpellService
    {
        private readonly IUnholyTransfusionSpellService unholyTransfuionSpellService;

        public UnholyNova(IGameStateService gameStateService,
            IModellingJournal journal,
            IUnholyTransfusionSpellService unholyTransfuionSpellService)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.UnholyNova;
            this.unholyTransfuionSpellService = unholyTransfuionSpellService;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.UnholyNova);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData, moreData);

            // Apply the transufion DoT/HoT
            var unholyTransfusionSpellData = gameStateService.GetSpellData(gameState, SpellIds.UnholyTransfusion);

            var uhtResults = unholyTransfuionSpellService.GetCastResults(gameState, unholyTransfusionSpellData);

            result.AdditionalCasts.Add(uhtResults);

            return result;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.UnholyNova);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            
            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.UnholyNova);

            var hastedCastTime = GetHastedCastTime(gameState, spellData, moreData);
            var hastedCd = GetHastedCooldown(gameState, spellData, moreData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            decimal maximumPotentialCasts = 60m / (hastedCastTime + hastedCd)
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
