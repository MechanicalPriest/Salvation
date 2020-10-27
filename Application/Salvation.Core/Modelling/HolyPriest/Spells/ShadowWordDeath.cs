using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class ShadowWordDeath : SpellService, IShadowWordDeathSpellService
    {
        public ShadowWordDeath(IGameStateService gameStateService)
            : base(gameStateService)
        {
            SpellId = (int)Spell.ShadowWordDeath;
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.ShadowWordDeath);

            var holyPriestAuraDamagesBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;
            var holyPriestAuraDamagesReduction = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
               .GetEffect(191079).BaseValue / 100 + 1;

            var damageSp = spellData.GetEffect(22171).SpCoefficient;

            var averageDamage = damageSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamagesBonus
                * holyPriestAuraDamagesReduction;

            double executePercent = _gameStateService.GetPlaystyle(gameState, "ShadowWordDeathPercentExecute").Value;
            double executeBonus = (spellData.GetEffect(340366).BaseValue / 100) - 1;

            // Check this math in the morning but percent of exe time multiplied by bonus should be right?
            averageDamage *= 1 + (executePercent * executeBonus);

            averageDamage *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return (averageDamage * GetNumberOfDamageTargets(gameState, spellData));
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.ShadowWordDeath);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);

            // A fix to the spell being modified to have no cast time and no gcd and no CD
            // This can happen if it's a component in another spell
            if (hastedCastTime == 0 && hastedGcd == 0 && hastedCd == 0)
                return 0;

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd);

            return maximumPotentialCasts;
        }

        public override double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.ShadowWordDeath);

            var spellDataRank2 = _gameStateService.GetSpellData(gameState, Spell.ShadowWordDeathRank2);
            
            // Base Val is neg
            // for some reason cd of death is a charge cd
            var baseCooldown = spellData.ChargeCooldown / 1000d + spellDataRank2.GetEffect(810248).BaseValue/1000d;

            return spellData.IsCooldownHasted
                ? baseCooldown / _gameStateService.GetHasteMultiplier(gameState)
                : baseCooldown;
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
