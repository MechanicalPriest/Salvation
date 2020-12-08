using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IForgeborneReveriesSpellService : ISpellService { }

    internal class ForgeborneReveries : SpellService, ISpellService<IForgeborneReveriesSpellService>
    {
        public ForgeborneReveries(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.LeadByExample;
        }

        public override double GetAverageIntellectBonus(GameState gameState, BaseSpellData spellData)
        {
            // Your $pri and Armor are increased by $<buff>% for each enchantment on your armor, up to $<max>%.
            // $buff=${$348272s1} $max=${3*$<buff>
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.ForgeborneReveriesBuff);

            var buffPerStack = buffSpellData.GetEffect(876233).BaseValue / 100;

            // This is hidden in spelldata as a multiplier as part of the $max variable.
            var maxStacks = 3;

            // TODO: Once enchants are implemented check if 3 exist. For now presume 3 because if you don't have 
            // 3 enchants trying to min/max is pretty silly anyway.

            return buffPerStack * maxStacks;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            // Value of 1 = 100% uptime
            return 1;
        }
    }
}
