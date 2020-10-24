using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;


namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Smite : SpellService, ISmiteSpellService
    {
        public Smite(IGameStateService gameStateService)
            : base(gameStateService)
        {
            SpellId = (int)Spell.Smite;
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Smite);

            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;

           
            var damageSp = spellData.GetEffect(236).SpCoefficient;
            var vers_multi = _gameStateService.GetVersatilityMultiplier(gameState);
            var intellect = _gameStateService.GetIntellect(gameState);
            double averageDmg = damageSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageDmg:0.##}");

            averageDmg *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            var smiteRank2 = _gameStateService.GetSpellData(gameState, Spell.SmiteRank2);

            averageDmg *= 1 + smiteRank2.GetEffect(624379).BaseValue / 100;

            return averageDmg * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Smite);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            double fillerCastTime = hastedCastTime == 0
                ? hastedGcd
                : hastedCastTime;

            double maximumPotentialCasts = 60d / fillerCastTime;

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
