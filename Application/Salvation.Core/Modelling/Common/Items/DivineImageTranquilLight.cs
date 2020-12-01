using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IDivineImageTranquilLightSpellService : ISpellService { }
    class DivineImageTranquilLight : SpellService, ISpellService<IDivineImageTranquilLightSpellService>
    {
        private readonly ISpellService<IRenewSpellService> _renewSpellService;

        public DivineImageTranquilLight(IGameStateService gameStateService,
            ISpellService<IRenewSpellService> renewSpellService)
            : base(gameStateService)
        {
            Spell = Spell.DivineImageTranquilLight;
            _renewSpellService = renewSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingSp = spellData.GetEffect(288956).SpCoefficient;

            // Overall healing for the HoT is: (220 + sp% * int) * int * vers * haste * crit * 6
            // Each tick is: (220 + sp% * int) * int * vers
            // Ticks is like a regular HoT: total_amount / tick_amount
            var baseTick = (220 + healingSp * _gameStateService.GetIntellect(gameState))
                * _gameStateService.GetVersatilityMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Base Tick: {baseTick:0.##}");

            var totalHealing = baseTick
                * _gameStateService.GetHasteMultiplier(gameState)
                * _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * 6;

            return totalHealing * GetNumberOfHealingTargets(gameState, spellData);
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
            var cpm = _renewSpellService.GetActualCastsPerMinute(gameState, null);

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
    }
}
