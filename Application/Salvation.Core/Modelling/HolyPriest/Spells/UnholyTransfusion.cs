using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class UnholyTransfusion : SpellService, IUnholyTransfusionSpellService
    {
        public UnholyTransfusion(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.UnholyTransfusion;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyTransfusion);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            // Flash Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            double averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            // Apply the Festering Transfusion conduit
            averageHeal *= GetFesteringTransfusionConduitMultiplier(gameState, spellData);
            var duration = GetDuration(gameState, spellData);

            // For each healing target, heal every ~1.5s for heal amt
            // TODO: Get a better number on healing events per player for the duration of UT
            return averageHeal * GetNumberOfHealingTargets(gameState, spellData) * (duration / 1.5d);
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyTransfusion);

            var holyPriestAuraDamageBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            // coeff2 * int * hpriest dmg mod * vers
            double averageDamage = spellData.Coeff2
                * _gameStateService.GetIntellect(gameState)
                * holyPriestAuraDamageBonus
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * 5; // Number of ticks

            _journal.Entry($"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##} (tick)");

            averageDamage *= _gameStateService.GetCriticalStrikeMultiplier(gameState);
            averageDamage *= _gameStateService.GetHasteMultiplier(gameState);

            // Apply the Festering Transfusion conduit
            averageDamage *= GetFesteringTransfusionConduitMultiplier(gameState, spellData);

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            return 0d;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyTransfusion);

            var baseDuration = base.GetDuration(gameState, spellData);

            // TODO: Shift this out to another method maybe, for testing?
            if (_gameStateService.IsConduitActive(gameState, Conduit.FesteringTransfusion))
            {
                var conduitData = _gameStateService.GetConduitData(gameState, Conduit.FesteringTransfusion);

                // The added duration is the same regardless of rank
                _journal.Entry($"[{spellData.Name}] Applying FesteringTransfusion ({(int)Conduit.FesteringTransfusion}) conduit " +
                    $"duration: {conduitData.Coeff1:0.##}");

                baseDuration += conduitData.Coeff1;
            }
            return baseDuration;
        }

        internal double GetFesteringTransfusionConduitMultiplier(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.UnholyTransfusion);

            if (_gameStateService.IsConduitActive(gameState, Conduit.FesteringTransfusion))
            {
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.FesteringTransfusion);
                var conduitData = _gameStateService.GetConduitData(gameState, Conduit.FesteringTransfusion);

                var multiplier = 1 + (conduitData.Ranks[rank] / 100);

                _journal.Entry($"[{spellData.Name}] Applying FesteringTransfusion ({(int)Conduit.FesteringTransfusion}) conduit " +
                    $"multiplier: {multiplier:0.##}");

                return multiplier;
            }

            return 1;
        }
    }
}
