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

        public override double GetAverageHealing(GameState gameState, BaseSpellData spellData)
        {
            // Max stacks
            var buffSpell = _gameStateService.GetSpellData(gameState, Spell.ValiantStrikesBuff);

            var maxStacks = buffSpell.MaxStacks;

            // TODO: Pull this from the overall model.
            var eventsPerMinute = _gameStateService.GetPlaystyle(gameState, "ValiantStrikesEventsPerMinute");

            if (eventsPerMinute == null)
                throw new ArgumentOutOfRangeException("ValiantStrikesEventsPerMinute", $"ValiantStrikesEventsPerMinute needs to be set.");

            var critsPerMinute = eventsPerMinute.Value * (_gameStateService.GetCriticalStrikeMultiplier(gameState) - 1);

            var playerHitpoints = _gameStateService.GetStamina(gameState);

            return Math.Min(maxStacks, critsPerMinute) * (playerHitpoints * 0.01);
        }
    }
}
