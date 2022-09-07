using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IThrillSeekerSpellService : ISpellService { }

    internal class ThrillSeeker : SpellService, ISpellService<IThrillSeekerSpellService>
    {
        public ThrillSeeker(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.ThrillSeeker;
        }

        public override double GetAverageHastePercent(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // While in combat, you gain a stack of Thrill Seeker every $t1 sec, or $s1 stacks on killing an enemy. 
            // At $331939u stacks Thrill Seeker is consumed to grant you Euphoria, increasing your Haste by $331937s1% for $331937d. 
            // Thrill Seeker decays rapidly while you are not in combat.

            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.ThrillSeekerBuff);

            var hasteAmount = buffSpellData.GetEffect(826391).BaseValue / 100;

            return hasteAmount * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.ThrillSeekerBuff);

            var duration = buffSpellData.Duration / 1000;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            return (GetActualCastsPerMinute(gameState, spellData) * GetDuration(gameState, spellData)) / 60;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var stackInterval = spellData.GetEffect(825885).Amplitude / 1000;

            var stacksSpellData = _gameStateService.GetSpellData(gameState, Spell.ThrillSeekerStacks);

            var stacksForBuff = stacksSpellData.MaxStacks;

            var timeForFullStacks = stacksForBuff * stackInterval;

            // The stacks start up again immediately, while the buff is active. No delay/icd
            return 60 / timeForFullStacks;
        }
    }
}
