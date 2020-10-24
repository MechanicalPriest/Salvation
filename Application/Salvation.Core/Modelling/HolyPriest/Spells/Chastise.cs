using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;


namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Chastise : SpellService, IChastiseSpellService
    {
        public Chastise(IGameStateService gameStateService)
            : base(gameStateService)
        {
            SpellId = (int)Spell.Chastise;
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Chastise);

            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;


            var damageSp = spellData.GetEffect(91044).SpCoefficient;
            var vers_multi = _gameStateService.GetVersatilityMultiplier(gameState);
            var intellect = _gameStateService.GetIntellect(gameState);
            double averageDmg = damageSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageDmg:0.##}");

            averageDmg *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageDmg * GetNumberOfDamageTargets(gameState, spellData);
        }

        // Doesnt account for holy words interaction
        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Chastise);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd);

            return maximumPotentialCasts;
        }

        public override double GetMinimumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

    }
}
