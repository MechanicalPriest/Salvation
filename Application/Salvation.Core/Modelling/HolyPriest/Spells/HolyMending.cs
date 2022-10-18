using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface IHolyMendingSpellService : ISpellService { }
    class HolyMending : SpellService, ISpellService<IHolyMendingSpellService>
    {
        public HolyMending(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.HolyMending;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healSpellData = _gameStateService.GetSpellData(gameState, Spell.HolyMendingHeal);

            var healingSp = healSpellData.GetEffect(1028476).SpCoefficient;

            var averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if (!spellData.Overrides.ContainsKey(Override.CastsPerMinute))
                throw new ArgumentOutOfRangeException("Override.CastsPerMinute", "SpellData Override.CastsPerMinute must be set.");

            var cpm = spellData.Overrides[Override.CastsPerMinute];

            return cpm;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            var healSpellData = _gameStateService.GetSpellData(gameState, Spell.HolyMendingHeal);

            return base.TriggersMastery(gameState, healSpellData);
        }
    }
}
