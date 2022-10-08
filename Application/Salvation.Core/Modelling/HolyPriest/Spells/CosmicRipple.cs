using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class CosmicRipple : SpellService, ISpellService<ICosmicRippleSpellService>
    {
        private readonly ISpellService<IHolyWordSerenitySpellService> _holyWordSerenitySpellService;
        private readonly ISpellService<IHolyWordSanctifySpellService> _holyWordSanctifySpellService;

        public CosmicRipple(IGameStateService gameStateService,
            ISpellService<IHolyWordSerenitySpellService> holyWordSerenitySpellService,
            ISpellService<IHolyWordSanctifySpellService> holyWordSanctifySpellService)
            : base(gameStateService)
        {
            Spell = Spell.CosmicRipple;
            _holyWordSerenitySpellService = holyWordSerenitySpellService;
            _holyWordSanctifySpellService = holyWordSanctifySpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healSpellData = _gameStateService.GetSpellData(gameState, Spell.CosmicRippleHeal);

            var healingSp = healSpellData.GetEffect(369498).SpCoefficient;

            double averageHeal = healingSp
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
            // Number of casts is the number of actual casts per minute from 
            // Sanc and Serenity. Ish.
            // TODO: Think about how to handle some minor variance due to it proccing at the end of their CD and not the start.

            var cpmSerenity = _holyWordSerenitySpellService.GetActualCastsPerMinute(gameState);
            var cpmSanctify = _holyWordSanctifySpellService.GetActualCastsPerMinute(gameState);

            return cpmSerenity + cpmSanctify;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            var cpmSerenity = _holyWordSerenitySpellService.GetMaximumCastsPerMinute(gameState);
            var cpmSanctify = _holyWordSanctifySpellService.GetMaximumCastsPerMinute(gameState);

            return cpmSerenity + cpmSanctify;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Ripple stores its number of targets in 360821.BaseValue
            var numTargets = spellData.GetEffect(360821).BaseValue;

            return numTargets;
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            var healSpellData = _gameStateService.GetSpellData(gameState, Spell.CosmicRippleHeal);

            return base.TriggersMastery(gameState, healSpellData);
        }
    }
}
