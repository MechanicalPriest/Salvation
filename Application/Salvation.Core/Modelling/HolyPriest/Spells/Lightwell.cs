using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Lightwell : SpellService, ISpellService<ILightwellSpellService>
    {
        public Lightwell(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.Lightwell;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var lightwellRenewSpelldata = _gameStateService.GetSpellData(gameState, Spell.LightwellHeal);

            var healingSp = lightwellRenewSpelldata.GetEffect(997691).SpCoefficient;

            double duration = lightwellRenewSpelldata.Duration / 1000;
            double tickrate = lightwellRenewSpelldata.GetEffect(997691).Amplitude / 1000;
            // HoT is affected by haste
            double averageHealTicks = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * (duration / tickrate);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Actual: {averageHealTicks:0.##} (ticks total)");
            
            // This just adds extra partial ticks.
            averageHealTicks *= _gameStateService.GetHasteMultiplier(gameState);

            // Add the rest of the multipliers
            averageHealTicks *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            return averageHealTicks * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            var chargesSpellData = _gameStateService.GetSpellData(gameState, Spell.LightwellCharges);

            var numCharges = chargesSpellData.MaxStacks;

            return numCharges;
        }
        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / hastedCd
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }
    }
}
