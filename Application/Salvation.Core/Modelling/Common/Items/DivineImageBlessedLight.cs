using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IDivineImageBlessedLightSpellService : ISpellService { }
    class DivineImageBlessedLight : SpellService, ISpellService<IDivineImageBlessedLightSpellService>
    {
        private readonly ISpellService<IPrayerOfMendingSpellService> _prayerOfMendingSpellService;

        public DivineImageBlessedLight(IGameStateService gameStateService,
            ISpellService<IPrayerOfMendingSpellService> prayerOfMendingSpellService)
            : base(gameStateService)
        {
            Spell = Spell.DivineImageBlessedLight;
            _prayerOfMendingSpellService = prayerOfMendingSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingSp = spellData.GetEffect(288952).SpCoefficient;

            var averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if (!spellData.Overrides.ContainsKey(Override.AllowedDuration))
                throw new ArgumentOutOfRangeException("Override.AllowedDuration", "SpellData Override.AllowedDuration must be set.");

            var allowedDuration = spellData.Overrides[Override.AllowedDuration];

            var maxCpm = GetMaximumCastsPerMinute(gameState, spellData);

            // Average number of casts per minute is casts per second increased by the number of seconds the buff is active
            return maxCpm / 60 * allowedDuration;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            var cpm = _prayerOfMendingSpellService.GetActualCastsPerMinute(gameState, null);

            return cpm;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            return spellData.GetEffect(336109).BaseValue;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
