using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IValiantStrikesSpellService : ISpellService { }
    public class ValiantStrikes : SpellService, ISpellService<IValiantStrikesSpellService>
    {
        public ValiantStrikes(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.ValiantStrikes;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            // Your crits create stacks up to 20. When ally drops below 50% they get healed for 1% of their max HP per stack. 
            // Easiest way to model it reasonably accurately will be: 
            // max_stacks: Maximum number of stacks you can hold at once
            // num_stacks_per_min: Number of stacks built up per minute
            // num_heals_per_minute: Number of events that try to eat some of the stacks
            // healing_per_event: MIN(max_stacks, num_stacks_per_min / num_heals_per_minute)
            // number_of_healing_events: config variable
            // Note: This doesn't really account well for stacks being chewed up so will need to be revisited with logs

            // Max stacks
            var buffSpell = _gameStateService.GetSpellData(gameState, Spell.ValiantStrikesBuff);

            var maxStacks = buffSpell.MaxStacks;

            // Number of stacks per minute
            // TODO: Pull this from the overall model.
            var eventsPerMinute = _gameStateService.GetPlaystyle(gameState, "ValiantStrikesEventsPerMinute");

            if (eventsPerMinute == null)
                throw new ArgumentOutOfRangeException("ValiantStrikesEventsPerMinute", $"ValiantStrikesEventsPerMinute needs to be set.");

            var critsPerMinute = eventsPerMinute.Value * (_gameStateService.GetCriticalStrikeMultiplier(gameState) - 1);

            // Healing amount
            var healingAmount = _gameStateService.GetHitpoints(gameState) * 0.01;

            // Heals per minute
            var healPpm = GetActualCastsPerMinute(gameState, spellData);

            return healingAmount * Math.Min(maxStacks, critsPerMinute / healPpm) * healPpm;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            var healPpm = _gameStateService.GetPlaystyle(gameState, "ValiantStrikesProcsPerMinute");

            if (healPpm == null)
                throw new ArgumentOutOfRangeException("ValiantStrikesProcsPerMinute", $"ValiantStrikesProcsPerMinute needs to be set.");

            return healPpm.Value;
        }
    }
}
