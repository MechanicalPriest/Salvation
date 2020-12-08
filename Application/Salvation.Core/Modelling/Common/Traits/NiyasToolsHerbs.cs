using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface INiyasToolsHerbsSpellService : ISpellService { }

    internal class NiyasToolsHerbs : SpellService, ISpellService<INiyasToolsHerbsSpellService>
    {
        public NiyasToolsHerbs(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.NiyasToolsHerbs;
        }

        public override double GetAverageHastePercent(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Your healing spells and abilities have a chance to apply Niya's Invigorating Herbs, 
            // increasing the target's Haste by $321510s1% for $321510d.
            // 321510 = NiyasToolsHerbsBuff

            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.NiyasToolsHerbsBuff);

            var hasteAmount = buffSpellData.GetEffect(809259).BaseValue / 100;

            return hasteAmount * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.NiyasToolsHerbsBuff);

            var duration = buffSpellData.Duration / 1000;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            // While the proc rate is 30% with 10s duration 10s ICD, the buff uptime is on average around 75-80%.
            var averageStacks = _gameStateService.GetPlaystyle(gameState, "NiyasToolsHerbsUptime");

            if (averageStacks == null)
                throw new ArgumentOutOfRangeException("NiyasToolsHerbsUptime", $"NiyasToolsHerbsUptime needs to be set.");

            return averageStacks.Value;
        }
    }
}
