using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface IFleshcraftSpellService : ISpellService { }
    public class Fleshcraft : SpellService, ISpellService<IFleshcraftSpellService>
    {
        public Fleshcraft(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.Fleshcraft;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Fleshcraft scales straight off HP. Shield = Health percent from 814354.
            var healthPercent = spellData.GetEffect(814354).BaseValue / 100;

            double averageHeal = healthPercent
                * _gameStateService.GetHitpoints(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            // Fleshcraft can absorb up to 1.5 times as much depending on the target.
            var extraAbsorb = _gameStateService.GetPlaystyle(gameState, "FleshCraftShieldMultiplier");

            if(extraAbsorb == null)
                throw new ArgumentOutOfRangeException("FleshCraftShieldMultiplier", $"FleshCraftShieldMultiplier needs to be set.");

            averageHeal *= extraAbsorb.Value;

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd)
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = spellData.Duration / 1000 / _gameStateService.GetHasteMultiplier(gameState);

            return hastedCastTime;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
