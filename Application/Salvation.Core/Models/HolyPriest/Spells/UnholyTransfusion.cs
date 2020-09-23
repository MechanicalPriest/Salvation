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
    public class UnholyTransfusion : SpellService, IUnholyTransfusionSpellService
    {
        public UnholyTransfusion(IGameStateService gameStateService,
            IModellingJournal journal)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.UnholyTransfusion;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.UnholyTransfusion);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            
            // Flash Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            // Apply the Festering Transfusion conduit
            averageHeal *= GetFesteringTransfusionConduitMultiplier(gameState, spellData);
            var duration = GetDuration(gameState, spellData);

            // For each healing target, heal every ~1.5s for heal amt
            // TODO: Get a better number on healing events per player for the duration of UT
            return averageHeal * spellData.NumberOfHealingTargets * (duration / 1.5m);
        }

        public override decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.UnholyTransfusion);

            var holyPriestAuraDamageBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            // coeff2 * int * hpriest dmg mod * vers
            decimal averageDamage = spellData.Coeff2
                * gameStateService.GetIntellect(gameState)
                * holyPriestAuraDamageBonus
                * gameStateService.GetVersatilityMultiplier(gameState)
                * 5; // Number of ticks

            journal.Entry($"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##} (tick)");

            averageDamage *= gameStateService.GetCriticalStrikeMultiplier(gameState);
            averageDamage *= gameStateService.GetHasteMultiplier(gameState);

            // Apply the Festering Transfusion conduit
            averageDamage *= GetFesteringTransfusionConduitMultiplier(gameState, spellData);

            return averageDamage * spellData.NumberOfDamageTargets;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            return 0m;
        }

        public override decimal GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.UnholyTransfusion);

            var baseDuration = base.GetDuration(gameState, spellData);

            // TODO: Shift this out to another method maybe, for testing?
            if (gameStateService.IsConduitActive(gameState, Conduit.FesteringTransfusion))
            {
                var conduitData = gameStateService.GetConduitData(gameState, Conduit.FesteringTransfusion);

                // The added duration is the same regardless of rank
                journal.Entry($"[{spellData.Name}] Applying FesteringTransfusion ({(int)Conduit.FesteringTransfusion}) conduit " +
                    $"duration: {conduitData.Coeff1:0.##}");

                baseDuration += conduitData.Coeff1;
            }
            return baseDuration;
        }

        internal decimal GetFesteringTransfusionConduitMultiplier(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.UnholyTransfusion);

            if (gameStateService.IsConduitActive(gameState, Conduit.FesteringTransfusion))
            {
                var rank = gameStateService.GetConduitRank(gameState, Conduit.FesteringTransfusion);
                var conduitData = gameStateService.GetConduitData(gameState, Conduit.FesteringTransfusion);

                var multiplier = 1 + (conduitData.Ranks[rank] / 100);

                journal.Entry($"[{spellData.Name}] Applying FesteringTransfusion ({(int)Conduit.FesteringTransfusion}) conduit " +
                    $"multiplier: {multiplier:0.##}");

                return multiplier;
            }

            return 1;
        }
    }
}
