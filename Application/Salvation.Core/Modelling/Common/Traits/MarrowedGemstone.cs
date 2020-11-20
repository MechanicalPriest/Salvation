using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IMarrowedGemstoneSpellService : ISpellService { }

    internal class MarrowedGemstone : SpellService, ISpellService<IMarrowedGemstoneSpellService>
    {

        public MarrowedGemstone(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.MarrowedGemstone;
        }

        public override double GetAverageCriticalStrikePercent(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // After landing ${$327066u+1} critical strikes, you gain $327069s1% increased chance to critical strike 
            // for $327069d. May only occur once per ${$327073d+$327069d} sec.
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.MarrowedGemstoneBuff);

            var critBonus = buffSpellData.GetEffect(818394).BaseValue / 100;

            return critBonus * GetUptime(gameState, spellData); ;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.MarrowedGemstoneBuff);

            var duration = buffSpellData.Duration / 1000;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Value of 1 = 100% uptime
            return GetActualCastsPerMinute(gameState, spellData) * GetDuration(gameState, spellData) / 60;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // CPM is 10s buff + 50s duration + time taken to build 10 stacks

            // TODO: Pull this from the model instead
            var numPotentialCritsPerMin = _gameStateService.GetPlaystyle(gameState, "MarrowedGemstoneEventsPerMinute");

            if (numPotentialCritsPerMin == null)
                throw new ArgumentOutOfRangeException("MarrowedGemstoneEventsPerMinute", $"MarrowedGemstoneEventsPerMinute needs to be set.");

            // Grab the max number of stacks
            var stacksSpellData = _gameStateService.GetSpellData(gameState, Spell.MarrowedGemstoneStacks);
            var maxStacks = stacksSpellData.MaxStacks + 1;

            // Get the number of events per second, figure out how many on average crit
            var critsPerSecond = (numPotentialCritsPerMin.Value / 60) * (_gameStateService.GetCriticalStrikeMultiplier(gameState, Spell) - 1);

            var secondsToMaxStacks = maxStacks / critsPerSecond;

            // Get the cooldown debuff data
            var debuffSpellData = _gameStateService.GetSpellData(gameState, Spell.MarrowedGemstoneCooldown);
            var debuffDuration = debuffSpellData.Duration / 1000;

            // time to stack + duration + cooldown / 60
            return 60 / (secondsToMaxStacks + GetDuration(gameState, spellData) + debuffDuration); 
        }
    }
}
